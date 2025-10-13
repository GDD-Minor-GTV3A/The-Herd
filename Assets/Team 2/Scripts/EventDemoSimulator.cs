using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
                    case KeyCode.R:
                        RefreshSheepList();
                        break;

                    // Sheep-Specific Events
                    case KeyCode.F:
                        ToggleSheepFreeze();
                        break;
                    case KeyCode.T:
                        TriggerThreatDetected();
                        break;
                    case KeyCode.H: // Help
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

        private void TriggerSheepUnfreeze()
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            _selectedSheep.OnSheepUnfreeze();
            DisplayEventFeedback($"<color=#00FFFFFF>Sheep Unfreeze</color>\nSheep: {_selectedSheep.name}\nState: Unfrozen");
            Debug.Log($"[EventDemo] SheepUnfreeze triggered on {_selectedSheep.name}");
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

        private void TriggerSeparatedFromHerd()
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            _selectedSheep.OnSeparatedFromHerd();
            DisplayEventFeedback($"<color=#FFA500FF>Separated From Herd</color>\nSheep: {_selectedSheep.name}");
            Debug.Log($"[EventDemo] SeparatedFromHerd triggered on {_selectedSheep.name}");
        }

        private void TriggerRejoinedHerd()
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            _selectedSheep.OnRejoinedHerd();
            DisplayEventFeedback($"<color=#00FF00FF>Rejoined Herd</color>\nSheep: {_selectedSheep.name}");
            Debug.Log($"[EventDemo] RejoinedHerd triggered on {_selectedSheep.name}");
        }

        private void TriggerPlayerAction(string actionType)
        {
            if (_selectedSheep == null)
            {
                DisplayEventFeedback("<color=red>No sheep selected!</color>\nPress Tab to select a sheep");
                return;
            }

            _selectedSheep.OnPlayerAction(actionType);
            DisplayEventFeedback($"<color=yellow>Player Action</color>\nSheep: {_selectedSheep.name}\nAction: {actionType}");
            Debug.Log($"[EventDemo] PlayerAction '{actionType}' triggered on {_selectedSheep.name}");
        }

        #endregion

        private void ShowHelp()
        {
            string help = @"<b>Sheep Event Demo Simulator</b>

<b><color=#00FFFFFF>SHEEP SELECTION</color></b>
<color=yellow>Tab</color> - Select Next Sheep
<color=yellow>R</color> - Refresh Sheep List

<b><color=#00FFFFFF>SHEEP EVENTS</color></b> (Selected Sheep)
<color=yellow>F</color> - Freeze Sheep
<color=yellow>T</color> - Threat Detected

<color=yellow>H</color> - Show This Help";

            DisplayEventFeedback(help, 10f);
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
            foreach (var sheep in _originalMaterials.Keys)
            {
                RemoveHighlight(sheep);
            }
            _originalMaterials.Clear();
        }
    }
}
