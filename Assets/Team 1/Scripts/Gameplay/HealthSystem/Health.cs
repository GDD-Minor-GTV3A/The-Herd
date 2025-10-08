using System;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// A universal health system for any entity.
    /// No longer a MonoBehaviour — just a pure class.
    /// </summary>
    public class Health
    {
        private float _maxHealth;
        private float _currentHealth;
        private bool _canTakeDamage;
        private bool _canBeHealed;
        private bool _canDie;

        // Events (replace UnityEvent with C# events)
        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnDeath;
        public event Action OnDamageTaken;
        public event Action OnHealed;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        public bool CanTakeDamage { get => _canTakeDamage; set => _canTakeDamage = value; }
        public bool CanBeHealed { get => _canBeHealed; set => _canBeHealed = value; }
        public bool CanDie { get => _canDie; set => _canDie = value; }

        public Health(float maxHealth = 100f, float currentHealth = 100f, bool canTakeDamage = true, bool canBeHealed = true, bool canDie = true)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            _canTakeDamage = canTakeDamage;
            _canBeHealed = canBeHealed;
            _canDie = canDie;
        }

        public void TakeDamage(float amount)
        {
            Debug.Log("Dealing 20 damage");
            if (!_canTakeDamage || amount <= 0) return;

            _currentHealth -= amount;
            _currentHealth = Math.Clamp(_currentHealth, 0, _maxHealth);

            OnDamageTaken?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0 && _canDie)
                Die();
        }

        public void Heal(float amount)
        {
            Debug.Log("Healing 10");
            if (!_canBeHealed || amount <= 0) return;

            _currentHealth += amount;
            _currentHealth = Math.Clamp(_currentHealth, 0, _maxHealth);

            OnHealed?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }

        public void SetMaxHealth(float newMax, bool resetToFull = true)
        {
            _maxHealth = Math.Max(1f, newMax);
            if (resetToFull)
                _currentHealth = _maxHealth;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

    }
}
