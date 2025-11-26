using UnityEngine;
using TMPro;
using Core.Events;
using Core.AI.Sheep.Event;
using System.Linq;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Tracks player sanity using a point-based system.
    /// Starts at 100 points with 6 sheep. Each sheep = 16 points.
    /// Losing 16 points removes furthest sheep. Gaining 16 points spawns new sheep.
    /// NOTE: This should probably be moved to player code for consistency.
    /// </summary>
    public class SanityTracker : MonoBehaviour
    {
        private const int POINTS_PER_SHEEP = 16;
        private const int STARTING_POINTS = 96;
        private const int STARTING_SHEEP_COUNT = 6;

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

        private int _sanityPoints;
        private int _maxSanityPoints;
        private SanityStage _currentStage;

        private static SanityTracker _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void OnEnable()
        {
            EventManager.AddListener<SheepDeathEvent>(OnSheepDeath);
            EventManager.AddListener<SheepJoinEvent>(OnSheepJoin);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SheepDeathEvent>(OnSheepDeath);
            EventManager.RemoveListener<SheepJoinEvent>(OnSheepJoin);
        }

        private void Start()
        {
            _sanityPoints = STARTING_POINTS;
            _maxSanityPoints = STARTING_POINTS;
            _currentStage = SanityStage.Stable;

            UpdateSanity();
        }

        /// <summary>
        /// Called when a sheep dies
        /// </summary>
        private void OnSheepDeath(SheepDeathEvent e)
        {
            //RemoveSanityPointsInternal(POINTS_PER_SHEEP);
        }

        /// <summary>
        /// Called when a sheep joins the herd
        /// </summary>
        private void OnSheepJoin(SheepJoinEvent e)
        {
            //AddSanityPointsInternal(POINTS_PER_SHEEP);
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

            _instance.AddSanityPointsInternal(points);
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

            _instance.RemoveSanityPointsInternal(points);
        }

        /// <summary>
        /// Internal method to add sanity points
        /// </summary>
        private void AddSanityPointsInternal(int points)
        {
            int oldPoints = _sanityPoints;

            // Add points and increase max
            if (_sanityPoints == _maxSanityPoints) _maxSanityPoints += points;
            _sanityPoints += points;
            
            int oldThreshold = oldPoints / POINTS_PER_SHEEP;
            int newThreshold = _sanityPoints / POINTS_PER_SHEEP;

            if (newThreshold > oldThreshold)
            {
                // Spawn a sheep (doesn't count towards sanity)
                SpawnSheep();

                var clip = _sanityAddSound;
                if (clip)
                {
                    float pitch = Random.Range(0.9f, 1.05f);
                    // Waiting for sound manager to roll out
                }
            }

            UpdateSanity();
        }

        /// <summary>
        /// Internal method to remove sanity points
        /// </summary>
        private void RemoveSanityPointsInternal(int points)
        {
            int oldPoints = _sanityPoints;

            _sanityPoints = Mathf.Max(0, _sanityPoints - points);
            
            // only changes when losing a full sheep's worth
            int oldThreshold = oldPoints > 0 ? Mathf.CeilToInt((float)oldPoints / POINTS_PER_SHEEP) : 0;
            int newThreshold = _sanityPoints > 0 ? Mathf.CeilToInt((float)_sanityPoints / POINTS_PER_SHEEP) : 0;

            if (newThreshold < oldThreshold && _sanityPoints > 0)
            {
                // Remove furthest sheep (TODO: sheep should flee instead of instant removal)
                RemoveFurthestSheep();

                var clip = _sanityRemoveSound;
                if (clip)
                {
                    //Waiting for sound manager
                }
            }

            UpdateSanity();
        }

        /// <summary>
        /// Spawns a sheep out of view, walking towards the player
        /// </summary>
        private void SpawnSheep()
        {
            if (_sheepPrefab == null)
            {
                Debug.LogWarning("[SanityTracker] No sheep prefab assigned. Cannot spawn sheep.");
                return;
            }

            if (_playerTransform == null)
            {
                Debug.LogWarning("[SanityTracker] No player transform assigned. Cannot spawn sheep.");
                return;
            }

            // Calculate spawn position out of view
            Vector3 spawnPosition = GetSpawnPositionOutOfView();

            // Instantiate sheep
            GameObject newSheepObj = Instantiate(_sheepPrefab, spawnPosition, Quaternion.identity);
            SheepStateManager newSheep = newSheepObj.GetComponent<SheepStateManager>();

            if (newSheep != null)
            {
                // Mark as straggler so it joins the herd
                newSheep.MarkAsStraggler();
                
                EventManager.Broadcast(new SheepJoinEvent(newSheep));

                Debug.Log($"[SanityTracker] Spawned sheep at {spawnPosition}");
            }
            else
            {
                Debug.LogError("[SanityTracker] Spawned sheep prefab does not have SheepStateManager component!");
            }
        }

        /// <summary>
        /// Calculates a spawn position out of the camera's view
        /// </summary>
        private Vector3 GetSpawnPositionOutOfView()
        {
            // Get a random direction behind or to the side of the player
            float randomAngle = Random.Range(90f, 270f); // Behind player (90-270 degrees from forward)
            Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * _playerTransform.forward;

            Vector3 spawnPos = _playerTransform.position + direction.normalized * _spawnDistance;

            // Ensure spawn position is on the ground (raycast down)
            if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
            {
                spawnPos = hit.point;
            }
            else
            {
                // If raycast fails, just use player's Y position
                spawnPos.y = _playerTransform.position.y;
            }

            return spawnPos;
        }

        /// <summary>
        /// Removes the sheep furthest from the player
        /// NOTE: For now removes instantly, but should make sheep flee instead
        /// </summary>
        private void RemoveFurthestSheep()
        {
            if (_playerTransform == null)
            {
                Debug.LogWarning("[SanityTracker] No player transform assigned. Cannot remove furthest sheep.");
                return;
            }

            // Find all sheep in the scene
            SheepStateManager[] allSheep = FindObjectsByType<SheepStateManager>(FindObjectsSortMode.None);

            if (allSheep.Length == 0)
            {
                Debug.LogWarning("[SanityTracker] No sheep found to remove.");
                return;
            }

            // Find the furthest sheep from player
            SheepStateManager furthestSheep = allSheep
                .OrderByDescending(sheep => Vector3.Distance(sheep.transform.position, _playerTransform.position))
                .FirstOrDefault();

            if (furthestSheep != null)
            {
                Debug.Log($"[SanityTracker] Removing furthest sheep: {furthestSheep.name}");
                // TODO: Make sheep flee instead of instant removal
                furthestSheep.Remove();
            }
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
                _debugText.text = $"Sanity: {percentage:F1}% ({_sanityPoints}/{_maxSanityPoints}) - {stageText}";
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
