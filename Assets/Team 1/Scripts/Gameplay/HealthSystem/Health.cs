using System;

using UnityEngine;

namespace Gameplay.HealthSystem
{
    /// <summary>
    /// A universal health system for any entity.
    /// This class is not a MonoBehaviour, so it can be used in plain C# or attached via a wrapper component.
    /// </summary>
    public class Health : IHealth
    {
        private float _maxHealth;
        private float _currentHealth;
        private bool _canTakeDamage;
        private bool _canBeHealed;
        private bool _canDie;

        // These use regular C# events instead of UnityEvents for flexibility and performance.
        public event Action<float, float> OnHealthChanged; // Invoked when health changes (current, max)
        public event Action OnDeath;                       // Invoked when entity dies
        public event Action OnDamageTaken;                 // Invoked when damage is applied
        public event Action OnHealed;                      // Invoked when healing is applied

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        public bool CanTakeDamage { get => _canTakeDamage; set => _canTakeDamage = value; }
        public bool CanBeHealed { get => _canBeHealed; set => _canBeHealed = value; }
        public bool CanDie { get => _canDie; set => _canDie = value; }

        /// <summary>
        /// Creates a new instance of Health with specified parameters.
        /// </summary>
        public Health(
            float maxHealth = 100f,
            float currentHealth = 100f,
            bool canTakeDamage = true,
            bool canBeHealed = true,
            bool canDie = true)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            _canTakeDamage = canTakeDamage;
            _canBeHealed = canBeHealed;
            _canDie = canDie;
        }

        /// <summary>
        /// Applies damage to the entity.
        /// </summary>
        public void TakeDamage(float amount)
        {
            Debug.Log($"Taking {amount} damage...");

            // Check if entity can take damage or amount is valid
            if (!_canTakeDamage || amount <= 0) return;

            // Subtract damage from current health
            _currentHealth -= amount;
            _currentHealth = Math.Clamp(_currentHealth, 0, _maxHealth);

            // Trigger events
            OnDamageTaken?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            // Handle death
            if (_currentHealth <= 0 && _canDie)
                Die();
        }

        /// <summary>
        /// Restores health to the entity.
        /// </summary>
        public void Heal(float amount)
        {
            Debug.Log($"Healing {amount}...");

            // Check if entity can be healed or amount is valid
            if (!_canBeHealed || amount <= 0) return;

            // Add health and clamp
            _currentHealth += amount;
            _currentHealth = Math.Clamp(_currentHealth, 0, _maxHealth);

            // Trigger events
            OnHealed?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Fully restores current health to max health.
        /// </summary>
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Updates the maximum health and optionally restores to full.
        /// </summary>
        public void SetMaxHealth(float newMax, bool resetToFull = true)
        {
            _maxHealth = Math.Max(1f, newMax);

            if (resetToFull)
                _currentHealth = _maxHealth;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Called internally when health reaches zero and death is enabled.
        /// </summary>
        private void Die()
        {
            Debug.Log("Entity has died.");
            OnDeath?.Invoke();
        }
    }
}
