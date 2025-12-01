using System;
using System.Collections;

using Core.AI.Sheep.Event;
using Core.Events;

using UnityEngine;
using UnityEngine.Serialization;


namespace Core.AI.Sheep
{
    [DisallowMultipleComponent]
    public sealed class SheepHealth : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SheepStateManager _sheep;

        [Header("Health")] [SerializeField] private int _maxHealthOverride = -1;
        [SerializeField] private bool _autoRemoveOnDeath = true;
        [SerializeField] private float _removeDelay = 3f;

        private int _currentHealth;
        private bool _isDead;

        public int MaxHealth => _maxHealthOverride > 0
        ? _maxHealthOverride
        : (_sheep?.Archetype?.MaxHealth ?? 100);
        
        public int CurrentHealth => _currentHealth;
        public bool IsDead => _isDead;

        private void Reset()
        {
            _sheep = GetComponent<SheepStateManager>();
        }

        private void Awake()
        {
            if (_sheep == null)
                _sheep = GetComponent<SheepStateManager>();

            _currentHealth = MaxHealth;
        }

        private void OnEnable()
        {
            EventManager.AddListener<SheepDamageEvent>(OnSheepDamageEvent);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SheepDamageEvent>(OnSheepDamageEvent);
        }

        private void OnSheepDamageEvent(SheepDamageEvent evt)
        {
            if (!_sheep || evt.Target != _sheep) return;
            ApplyDamage(evt.Amount);
        }

        #region API

        public void ApplyDamage(float amount)
        {
            if (_isDead) return;
            if (amount <= 0f) return;
            
            int amountInt = Mathf.CeilToInt(amount);
            
            int oldHealth = _currentHealth;
            int newHealth = Mathf.Max(0, _currentHealth - amountInt);
            
            if (newHealth == oldHealth) return;
            _currentHealth = newHealth;
            
            EventManager.Broadcast(new SheepDamagedEvent(_sheep, oldHealth, newHealth, MaxHealth));
            EventManager.Broadcast(new SheepHealthChangedEvent(_sheep, _currentHealth, MaxHealth));

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            if (_isDead) return;
            if (amount <= 0) return;

            int oldHealth = _currentHealth;
            int newHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
            
            if (newHealth == oldHealth) return;
            _currentHealth = newHealth;
            
            EventManager.Broadcast(new SheepHealthChangedEvent(_sheep, _currentHealth, MaxHealth));
        }
        
        #endregion

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            if (_sheep != null)
            {
                _sheep.OnSheepDie();
            }

            if (_autoRemoveOnDeath && _sheep != null)
            {
                StartCoroutine(RemoveAfterDelay());
            }
        }

        private IEnumerator RemoveAfterDelay()
        {
            yield return new WaitForSeconds(_removeDelay);
            if (_sheep != null)
            {
                _sheep.Remove();
            }
        }
    }
}
