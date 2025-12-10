using UnityEngine;
using TMPro;
using Core.Events;
using Core.AI.Sheep.Event;
using System;
using System.Linq;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Distance zone configuration for sanity system
    /// </summary>
    [Serializable]
    public class SanityDistanceZone
    {
        [Tooltip("Maximum radius for this zone in meters")]
        public float maxRadius;

        [Tooltip("Sanity points per second per sheep in this zone")]
        public float sanityRatePerSecond;

        [Tooltip("Zone name for debugging")]
        public string zoneName = "Zone";
    }

    /// <summary>
    /// Tracks player sanity using a distance-based system.
    /// Sheep distance from player affects sanity over time.
    /// Closer sheep increase sanity, farther sheep decrease it.
    /// </summary>
    public class SanityTracker : MonoBehaviour
    {
        private const int STARTING_SHEEP_COUNT = 5;
        private const int POINTS_PER_SHEEP = 100 / STARTING_SHEEP_COUNT;
        private const int STARTING_POINTS = POINTS_PER_SHEEP * STARTING_SHEEP_COUNT;

        [Header("Spawning")]
        [SerializeField]
        [Tooltip("Sheep prefab to spawn when gaining sanity")]
        private GameObject _sheepPrefab;

        [SerializeField]
        [Tooltip("Player transform for spawn positioning")]
        private Transform _playerTransform;

        [SerializeField]
        [Tooltip("Distance behind/beside player to spawn sheep (out of view)")]
        private float _spawnDistance = 15f;

        [Header("Debug")]
        [SerializeField]
        [Tooltip("Enable on-screen debug display")]
        private bool _showDebug = false;

        [SerializeField]
        [Tooltip("TextMeshPro component for debug display")]
        private TextMeshProUGUI _debugText;
        
        [Header("Audio")] [SerializeField] private AudioClip _sanityAddSound;
        [SerializeField] private AudioClip _sanityRemoveSound;

        [Header("Distance-Based Sanity")]
        [SerializeField]
        [Tooltip("Enable distance-based sanity system")]
        private bool _enableDistanceBasedSanity = true;

        [SerializeField]
        [Tooltip("Define zones from closest to furthest. Each sheep in a zone contributes its rate.")]
        private SanityDistanceZone[] _sanityZones =
        {
            new(){ zoneName = "Safe", maxRadius = 15f, sanityRatePerSecond = 2f },
            new(){ zoneName = "Warning", maxRadius = 30f, sanityRatePerSecond = 0f },
            new(){zoneName = "Dangerous", maxRadius = 45f, sanityRatePerSecond = -1f },
            new(){ zoneName = "Death", maxRadius = 60f, sanityRatePerSecond = -2f },
            new(){ zoneName = "Outside", maxRadius = float.MaxValue, sanityRatePerSecond = -3f }
        };

        [Header("Escalation Settings")]
        [SerializeField]
        [Tooltip("Which zone index (0-based) should have escalation. -1 to disable.")]
        private int _escalationZoneIndex = 4;

        [SerializeField]
        [Tooltip("Additional sanity change per second that accumulates each second (e.g., -1 means it gets -1 worse each second)")]
        private float _escalationRatePerSecond = -1f;

        [Header("Performance")]
        [SerializeField]
        [Tooltip("Update interval in seconds (0 = every frame)")]
        private float _updateInterval = 0.1f;

        private float _sanityPoints;
        private float _maxSanityPoints;
        private SanityStage _currentStage;

        // Distance-based sanity tracking
        private float _timeSinceLastUpdate = 0f;
        private float _escalationZoneEntryTime = -1f;
        private bool _anySheepInEscalationZone = false;

        private static SanityTracker _instance;

        // --------- public getters ----------
        public static float CurrentPoints =>
            _instance != null ? _instance._sanityPoints : 0f;

        public static float MaxPoints =>
            _instance != null ? _instance._maxSanityPoints : 0f;

        public static float CurrentPercentage =>
            _instance != null && _instance._maxSanityPoints > 0
                ? (_instance._sanityPoints / _instance._maxSanityPoints) * 100f
                : 0f;

        public static SanityStage CurrentStage =>
            _instance != null ? _instance._currentStage : SanityStage.Stable;
        
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _sanityPoints = STARTING_POINTS;
            _maxSanityPoints = STARTING_POINTS;
            _currentStage = SanityStage.Stable;

            UpdateSanity();
        }

        private void Update()
        {
            if (!_enableDistanceBasedSanity) return;

            _timeSinceLastUpdate += Time.deltaTime;

            if (_timeSinceLastUpdate >= _updateInterval)
            {
                float deltaTime = _timeSinceLastUpdate;
                _timeSinceLastUpdate = 0f;

                float sanityDelta = CalculateSanityDeltaForInterval(deltaTime);

                if (Mathf.Abs(sanityDelta) > 0.001f)
                {
                    ApplySanityDelta(sanityDelta);
                }
            }
        }

        /// <summary>
        /// Calculates sanity change based on all sheep distances from player
        /// </summary>
        private float CalculateSanityDeltaForInterval(float deltaTime)
        {
            if (_playerTransform == null) return 0f;

            float totalDelta = 0f;
            Vector3 playerPos = _playerTransform.position;
            int furthestZoneSheepCount = 0;

            // Get all active sheep
            SheepStateManager[] allSheep = FindObjectsByType<SheepStateManager>(FindObjectsSortMode.None);

            foreach (var sheep in allSheep)
            {
                if (sheep == null || !sheep.isActiveAndEnabled) continue;

                // Calculate distance to player
                float distance = Vector3.Distance(sheep.transform.position, playerPos);

                // Determine which zone this sheep is in
                int zoneIndex = GetZoneIndexForDistance(distance);

                if (zoneIndex >= 0 && zoneIndex < _sanityZones.Length)
                {
                    float baseRate = _sanityZones[zoneIndex].sanityRatePerSecond;

                    // Track the furthest zone occupancy for escalation
                    if (zoneIndex == _escalationZoneIndex)
                    {
                        furthestZoneSheepCount++;
                    }

                    totalDelta += baseRate * deltaTime;
                }
            }

            // Update escalation tracking
            bool wasInEscalationZone = _anySheepInEscalationZone;
            _anySheepInEscalationZone = furthestZoneSheepCount > 0;

            if (!wasInEscalationZone && _anySheepInEscalationZone)
            {
                // First sheep entered escalation zone
                _escalationZoneEntryTime = Time.time;
            }
            else if (wasInEscalationZone && !_anySheepInEscalationZone)
            {
                // All sheep left escalation zone
                _escalationZoneEntryTime = -1f;
            }

            // Add escalation penalty
            if (_anySheepInEscalationZone && _escalationZoneIndex >= 0)
            {
                float escalationPenalty = CalculateEscalationPenalty();
                totalDelta += escalationPenalty * furthestZoneSheepCount * deltaTime;
            }

            return totalDelta;
        }

        /// <summary>
        /// Determines which zone a sheep is in based on distance
        /// </summary>
        private int GetZoneIndexForDistance(float distance)
        {
            for (int i = 0; i < _sanityZones.Length; i++)
            {
                if (distance <= _sanityZones[i].maxRadius)
                {
                    return i;
                }
            }

            // If distance exceeds all zones, return last zone
            return _sanityZones.Length - 1;
        }

        /// <summary>
        /// Calculates additional penalty based on how long sheep have been in escalation zone
        /// Escalates linearly: baseRate + (escalationRate * timeInSeconds)
        /// Example: -3/sec base + (-1/sec escalation) after 5 seconds = -3 + (-1*5) = -8/sec
        /// </summary>
        private float CalculateEscalationPenalty()
        {
            if (_escalationZoneEntryTime < 0f) return 0f;

            float timeInZone = Time.time - _escalationZoneEntryTime;

            // Linear escalation: multiply rate by time in zone
            return _escalationRatePerSecond * timeInZone;
        }

        /// <summary>
        /// Static method to add sanity points from external systems
        /// </summary>
        public static void AddSanityPoints(int points)
        {
            if (_instance == null)
            {
                Debug.LogWarning("[SanityTracker] No instance found. Cannot add sanity points.");
                return;
            }

            _instance.ApplySanityDelta(points);
        }

        /// <summary>
        /// Static method to remove sanity points from external systems
        /// </summary>
        public static void RemoveSanityPoints(int points)
        {
            if (_instance == null)
            {
                Debug.LogWarning("[SanityTracker] No instance found. Cannot remove sanity points.");
                return;
            }

            _instance.ApplySanityDelta(-(float)points);
        }

        /// <summary>
        /// Applies a change to sanity points (positive or negative)
        /// </summary>
        private void ApplySanityDelta(float delta)
        {
            // If gaining sanity while at max, increase the ceiling
            if (delta > 0 && _sanityPoints >= _maxSanityPoints - 0.1f)
            {
                _maxSanityPoints += delta;
            }

            // Apply change
            _sanityPoints = Mathf.Clamp(_sanityPoints + delta, 0f, _maxSanityPoints);

            UpdateSanity();
        }

        /// <summary>
        /// Calculates and broadcasts sanity percentage and stage
        /// </summary>
        private void UpdateSanity()
        {
            float percentage = (_sanityPoints / (float)_maxSanityPoints) * 100f;
            
            EventManager.Broadcast(new SanityChangeEvent(percentage));

            // Check for stage change
            SanityStage newStage = GetSanityStage(percentage);
            if (newStage != _currentStage)
            {
                SanityStage oldStage = _currentStage;
                _currentStage = newStage;
                EventManager.Broadcast(new SanityStageChangeEvent(oldStage, newStage));
                Debug.Log($"[SanityTracker] Stage changed: {oldStage} -> {newStage}");
            }
            
            if (_showDebug && _debugText != null)
            {
                string stageText = GetStageName(_currentStage);
                _debugText.text = $"Sanity: {percentage:F1}% ({_sanityPoints:F1}/{_maxSanityPoints:F1}) - {stageText}";

                if (_anySheepInEscalationZone && _escalationZoneEntryTime >= 0f)
                {
                    float escalationTime = Time.time - _escalationZoneEntryTime;
                    float penalty = CalculateEscalationPenalty();
                    _debugText.text += $"\nEscalation: {escalationTime:F1}s (penalty: {penalty:F1}/s)";
                }
            }
        }

        /// <summary>
        /// Determines sanity stage based on percentage
        /// </summary>
        private SanityStage GetSanityStage(float percentage)
        {
            return percentage switch
            {
                <= 0f => SanityStage.Death,
                < 25f => SanityStage.BreakingPoint,
                < 50f => SanityStage.Unstable,
                < 75f => SanityStage.Fragile,
                _ => SanityStage.Stable
            };
        }

        /// <summary>
        /// Gets the display name for a sanity stage
        /// </summary>
        private string GetStageName(SanityStage stage)
        {
            return stage switch
            {
                SanityStage.Stable => "Stable",
                SanityStage.Fragile => "Fragile",
                SanityStage.Unstable => "Unstable",
                SanityStage.BreakingPoint => "Breaking Point",
                SanityStage.Death => "Death",
                _ => "Unknown"
            };
        }
    }
}
