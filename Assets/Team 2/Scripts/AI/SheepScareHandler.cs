using System;

using Core.AI.Sheep.Event;
using Core.Events;

using UnityEngine;

namespace Core.AI.Sheep
{
    [RequireComponent(typeof(SheepStateManager))]
    [DisallowMultipleComponent]
    public class SheepScareHandler : MonoBehaviour
    {
        [Header("Scare settings")] [SerializeField]
        private float _scareThreshold = 10f;
        [SerializeField] private float _scareDecayRate = 1.0f;
        [SerializeField] private float _cooldownAfterPanic = 5.0f;

        [Header("Debug")] [SerializeField] private float _currentScareValue;
        [SerializeField] private bool _isPanicking;

        private SheepStateManager _sheep;
        private float _nextCanPanicTime;
        
        public Vector3 LastScareSource { get; private set; }

        private void Awake()
        {
            _sheep = GetComponent<SheepStateManager>();
        }

        private void OnEnable()
        {
            EventManager.AddListener<SheepScareEvent>(OnSheepScare);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SheepScareEvent>(OnSheepScare);
        }

        private void Update()
        {
            if (_currentScareValue > 0 && !_isPanicking)
            {
                _currentScareValue -= _scareDecayRate * Time.deltaTime;
                _currentScareValue = Mathf.Max(0, _currentScareValue);
            }

            if (_isPanicking)
            {
                if (!(_sheep.GetState() is SheepScaredState))
                {
                    //Debug.Log($"[SheepScareHandler] {_sheep.name} panic interrupted by state {_sheep.GetState().GetType().Name}");
                    _isPanicking = false;
                    _currentScareValue = 0f;
                    _nextCanPanicTime = Time.time + _cooldownAfterPanic;
                }
            }
        }

        private void OnSheepScare(SheepScareEvent evt)
        {
            if (evt.Target != _sheep) return;
            
            if (_sheep.GetState() is SheepDieState || _isPanicking || Time.time < _nextCanPanicTime) return;
            LastScareSource = evt.SourcePosition;
            AddFear(evt.Amount);
        }

        public void AddFear(float amount)
        {
            _currentScareValue += amount;

            if (_currentScareValue >= _scareThreshold)
            {
                TriggerPanic();
            }
        }

        private void TriggerPanic()
        {
            if (_isPanicking) return;
            
            Debug.Log($"[SheepScareHandler] {_sheep.name} reached threshold");
            _isPanicking = true;
            
            _sheep.SetState<SheepScaredState>();
        }
    }
}
