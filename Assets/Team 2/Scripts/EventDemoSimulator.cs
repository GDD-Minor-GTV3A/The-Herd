using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Core.Events;
using Core.AI.Sheep.Event;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Demo script to simulate sheep-specific events via keyboard shortcuts
    /// Allows selecting individual sheep to trigger events on them
    /// </summary>
    public class EventDemoSimulator : MonoBehaviour
    {
        [Header("UI Display")]
        [SerializeField]
        [Tooltip("Text component to display event notifications")]
        private TextMeshProUGUI _feedbackText;

        [SerializeField]
        [Tooltip("How long to display each event notification (seconds)")]
        private float _displayDuration = 2f;
        
        private List<SheepStateManager> _allSheep = new List<SheepStateManager>();

        [SerializeField]
        [Tooltip("Currently selected sheep for targeted events")]
        private SheepStateManager _selectedSheep;

        [SerializeField]
        [Tooltip("Material to highlight selected sheep")]
        private Material _highlightMaterial;

        [Header("Threat Settings")]
        [SerializeField]
        [Tooltip("Offset from selected sheep for threat position")]
        private Vector3 _threatOffset = new Vector3(3f, 0f, 0f);

        [Header("Spawn Settings")]
        [SerializeField]
        [Tooltip("Sheep prefab to spawn")]
        private GameObject _sheepPrefab;

        [SerializeField]
        [Tooltip("Player transform for spawn position")]
        private Transform _playerTransform;

        [SerializeField]
        [Tooltip("Offset from player for spawn position")]
        private Vector3 _spawnOffset = new Vector3(2f, 2f, 0f);

        [Header("Sanity Settings")]
        [SerializeField]
        [Tooltip("Amount of sanity points to add/remove per key press")]
        private int _sanityPointsChange = 2;

        private int _selectedSheepIndex = 0;
        private float _hideTextAt;
        private string _currentEventText = "";
        private Dictionary<SheepStateManager, Material> _originalMaterials = new Dictionary<SheepStateManager, Material>();

        private void Start()
        {
            RefreshSheepList();

            if (_allSheep.Count > 0)
            {
                SelectSheep(0);
            }
        }

        private void Update()
        {
            // Hide feedback text after duration
            if (_feedbackText != null && Time.time >= _hideTextAt && !string.IsNullOrEmpty(_currentEventText))
            {
                _currentEventText = "";
                UpdateFeedbackDisplay();
            }

            // Check for keyboard shortcuts
            HandleKeyboardInput();
        }

        private void HandleKeyboardInput()
        {
            // Check each key press
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key)) continue;

                switch (key)
                {
                    // Sheep Selection
                    case KeyCode.Tab:
                        SelectNextSheep();
                        break;

                    // Sheep-Specific Events
                    case KeyCode.F:
                        ToggleSheepFreeze();
                        break;
                    case KeyCode.T:
                        TriggerThreatDetected();
                        break;
                    case KeyCode.K:
                        KillSelectedSheep();
                        break;
                    case KeyCode.J:
                        SpawnSheep();
                        break;

                    // Sanity Control
                    case KeyCode.Equals:
                    case KeyCode.Plus:
                    case KeyCode.KeypadPlus:
                        IncreaseSanity();
                        break;
                    case KeyCode.Minus:
                    case KeyCode.KeypadMinus:
                        DecreaseSanity();
                        break;

                    case KeyCode.H:
                        ShowHelp();
                        break;
                }
            }
        }

        #region Sheep Selection

        private void RefreshSheepList()
        {
            _allSheep.Clear();
            _allSheep.AddRange(FindObjectsOfType<SheepStateManager>());

            DisplayEventFeedback($"<color=white>Sheep List Refreshed</color>\nFound {_allSheep.Count} sheep", 1.5f);
            Debug.Log($"[EventDemo] Refreshed sheep list. Found {_allSheep.Count} sheep");

            if (_allSheep.Count > 0 && (_selectedSheep == null || !_allSheep.Contains(_selectedSheep)))
            {
                SelectSheep(0);
            }
        }

        private void SelectNextSheep()
        {
            if (_allSheep.Count == 0) return;
            _selectedSheepIndex = (_selectedSheepIndex + 1) % _allSheep.Count;
            SelectSheep(_selectedSheepIndex);
        }

        private void SelectSheep(int index)
        {
            if (_allSheep.Count == 0) return;
            if (index < 0 || index >= _allSheep.Count) return;

            // Remove highlight from previous sheep
            if (_selectedSheep != null)
            {
                RemoveHighlight(_selectedSheep);
            }

            _selectedSheepIndex = index;
            _selectedSheep = _allSheep[index];

            // Highlight new sheep
            if (_selectedSheep != null)
            {
                ApplyHighlight(_selectedSheep);
                DisplayEventFeedback($"<color=#00FFFFFF>Sheep Selected</color>\n{_selectedSheep.name}\n({_selectedSheepIndex + 1}/{_allSheep.Count})", 1.5f);
                Debug.Log($"[EventDemo] Selected sheep: {_selectedSheep.name} ({_selectedSheepIndex + 1}/{_allSheep.Count})");
            }
        }

        private void ApplyHighlight(SheepStateManager sheep)
        {
            if (sheep == null || _highlightMaterial == null) return;

            Renderer renderer = sheep.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                if (!_originalMaterials.ContainsKey(sheep))
                {
                    _originalMaterials[sheep] = renderer.material;
                }
                renderer.material = _highlightMaterial;
            }
        }

        private void RemoveHighlight(SheepStateManager sheep)
        {
            if (sheep == null) return;

            Renderer renderer = sheep.GetComponentInChildren<Renderer>();
            if (renderer != null && _originalMaterials.ContainsKey(sheep))
            {
                renderer.material = _originalMaterials[sheep];
                _originalMaterials.Remove(sheep);
            }
        }

        #endregion

        #region Sheep-Specific Events

        private void ToggleSheepFreeze()
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            string state;
            if (_selectedSheep.GetState() is SheepFreezeState)
            {
                state = "Unfreeze";
                _selectedSheep.SetState<SheepGrazeState>();
            }
            else
            {
                state = "Freeze";
                _selectedSheep.OnSheepFreeze();
            }
            
            DisplayEventFeedback($"<color=#00FFFFFF>Sheep {state}</color>\nSheep: {_selectedSheep.name}");
            Debug.Log($"[EventDemo] Sheep{state} triggered on {_selectedSheep.name}");
        }

        private void TriggerThreatDetected()
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            Vector3 threatPosition = _selectedSheep.transform.position + _threatOffset;
            _selectedSheep.OnThreatDetected(threatPosition);

            DisplayEventFeedback($"<color=red>Threat Detected</color>\nSheep: {_selectedSheep.name}\nThreat at: {threatPosition}");
            Debug.Log($"[EventDemo] ThreatDetected triggered on {_selectedSheep.name} at position {threatPosition}");
        }

        private void KillSelectedSheep()
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            string sheepName = _selectedSheep.name;
            _selectedSheep.Remove();
            _allSheep.Remove(_selectedSheep);
            _selectedSheep = null;

            DisplayEventFeedback($"<color=red>Sheep Death</color>\nKilled: {sheepName}");
            Debug.Log($"[EventDemo] Sheep killed: {sheepName}");

            // Select next sheep if available
            if (_allSheep.Count > 0)
            {
                _selectedSheepIndex = Mathf.Min(_selectedSheepIndex, _allSheep.Count - 1);
                SelectSheep(_selectedSheepIndex);
            }
        }

        private void SpawnSheep()
        {
            if (_sheepPrefab == null)
            {
                DisplayEventFeedback("<color=red>No sheep prefab assigned!</color>\nAssign prefab in inspector");
                return;
            }

            if (_playerTransform == null)
            {
                DisplayEventFeedback("<color=red>No player transform assigned!</color>\nAssign transform in inspector");
                return;
            }

            Vector3 spawnPosition = _playerTransform.position + _playerTransform.forward * _spawnOffset.z + _playerTransform.right * _spawnOffset.x + Vector3.up * _spawnOffset.y;

            GameObject newSheepObj = Instantiate(_sheepPrefab, spawnPosition, Quaternion.identity);
            SheepStateManager newSheep = newSheepObj.GetComponent<SheepStateManager>();

            if (newSheep != null)
            {
                _allSheep.Add(newSheep);
                EventManager.Broadcast(new SheepJoinEvent(newSheep));
                DisplayEventFeedback($"<color=#00FF00FF>Sheep Spawned</color>\nName: {newSheepObj.name}\nPosition: {spawnPosition}");
            }
            else
            {
                DisplayEventFeedback("<color=red>Spawned object has no SheepStateManager!</color>");
                Debug.LogError("[EventDemo] Spawned sheep prefab does not have SheepStateManager component!");
            }
        }

        #endregion

        #region Sanity Control

        private void IncreaseSanity()
        {
            SanityTracker.AddSanityPoints(_sanityPointsChange);
            DisplayEventFeedback($"<color=#00FF00FF>Sanity Increased</color>\nAdded {_sanityPointsChange} points");
            Debug.Log($"[EventDemo] Added {_sanityPointsChange} sanity points");
        }

        private void DecreaseSanity()
        {
            SanityTracker.RemoveSanityPoints(_sanityPointsChange);
            DisplayEventFeedback($"<color=#FF0000FF>Sanity Decreased</color>\nRemoved {_sanityPointsChange} points");
            Debug.Log($"[EventDemo] Removed {_sanityPointsChange} sanity points");
        }

        #endregion

        private void ShowHelp()
        {
            string help = @"<b>Sheep Event Demo Simulator</b>

<b><color=#00FFFFFF>SHEEP SELECTION</color></b>
<color=yellow>Tab</color> - Select Next Sheep

<b><color=#00FFFFFF>SHEEP EVENTS</color></b> (Selected Sheep)
<color=yellow>F</color> - Freeze Sheep
<color=yellow>T</color> - Threat Detected
<color=yellow>K</color> - Kill Sheep (Death Event)
<color=yellow>J</color> - Spawn Sheep (Join Event)

<b><color=#00FFFFFF>SANITY CONTROL</color></b>
<color=yellow>+</color> - Increase Sanity Points
<color=yellow>-</color> - Decrease Sanity Points

<color=yellow>H</color> - Show This Help";

            DisplayEventFeedback(help, 15f);
            Debug.Log("[EventDemo] Help displayed");
        }

        private void DisplayEventFeedback(string message, float duration = -1f)
        {
            _currentEventText = message;
            _hideTextAt = Time.time + (duration > 0 ? duration : _displayDuration);
            UpdateFeedbackDisplay();
        }

        private void UpdateFeedbackDisplay()
        {
            if (_feedbackText != null)
            {
                _feedbackText.text = _currentEventText;
            }
        }

        private void OnEnable()
        {
            // Show help on start
            Invoke(nameof(ShowHelp), 0.5f);
        }

        private void OnDisable()
        {
            // Remove all highlights
            var copy = _originalMaterials.Keys;
            foreach (var sheep in copy)
            {
                RemoveHighlight(sheep);
            }
            _originalMaterials.Clear();
        }
    }
}
