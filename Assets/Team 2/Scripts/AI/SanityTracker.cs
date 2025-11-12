using UnityEngine;
using TMPro;
using Core.Events;
using Core.AI.Sheep.Event;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Tracks player sanity based on sheep count.
    /// NOTE: This should probably be moved to player code for consistency.
    /// </summary>
    public class SanityTracker : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Maximum number of sheep in the herd")]
        private int _maxSheepCount = 10;

        [Header("Debug")]
        [SerializeField]
        [Tooltip("Enable on-screen debug display")]
        private bool _showDebug = false;

        [SerializeField]
        [Tooltip("TextMeshPro component for debug display")]
        private TextMeshProUGUI _debugText;

        private int _currentSheepCount;

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
            _currentSheepCount = FindObjectsByType<SheepStateManager>(FindObjectsSortMode.None).Length;
            _maxSheepCount = Mathf.Max(_maxSheepCount, _currentSheepCount);
            UpdateSanity();
        }

        /// <summary>
        /// Called when a sheep dies
        /// </summary>
        private void OnSheepDeath(SheepDeathEvent e)
        {
            _currentSheepCount = Mathf.Max(0, _currentSheepCount - 1);
            UpdateSanity();
        }

        /// <summary>
        /// Called when a sheep joins the herd
        /// </summary>
        private void OnSheepJoin(SheepJoinEvent e)
        {
            _currentSheepCount = Mathf.Min(_maxSheepCount, _currentSheepCount + 1);
            UpdateSanity();
        }

        /// <summary>
        /// Calculates and broadcasts sanity percentage
        /// </summary>
        private void UpdateSanity()
        {
            float percentage = (_currentSheepCount / (float)_maxSheepCount) * 100f;
            EventManager.Broadcast(new SanityChangeEvent(percentage));

            if (_showDebug && _debugText != null)
            {
                _debugText.text = $"Sanity: {percentage:F1}% ({_currentSheepCount}/{_maxSheepCount})";
            }
        }

        /// <summary>
        /// Sets the maximum sheep count
        /// </summary>
        public void SetMaxSheepCount(int maxCount)
        {
            _maxSheepCount = Mathf.Max(1, maxCount);
            UpdateSanity();
        }

        /// <summary>
        /// Sets the current sheep count
        /// </summary>
        public void SetCurrentSheepCount(int currentCount)
        {
            _currentSheepCount = Mathf.Clamp(currentCount, 0, _maxSheepCount);
            UpdateSanity();
        }
    }
}
