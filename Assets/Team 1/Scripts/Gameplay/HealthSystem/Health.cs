using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    /// <summary>
    /// A universal health system for any entity (player, enemy, object).
    /// Supports damage, healing, and death control flags.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField, Tooltip("Total hitpoints for this entity.")]
        private float _maxHealth = 100f;

        [SerializeField, Tooltip("Current hitpoints at runtime.")]
        private float _currentHealth = 100f;

        [Header("Health Behavior")]
        [SerializeField, Tooltip("Can this entity take damage?")]
        private bool _canTakeDamage = true;

        [SerializeField, Tooltip("Can this entity be healed?")]
        private bool _canBeHealed = true;

        [SerializeField, Tooltip("Can this entity die when health reaches 0?")]
        private bool _canDie = true;

        [Header("Events")]
        public UnityEvent<float, float> OnHealthChanged; // (current, max)
        public UnityEvent OnDeath;
        public UnityEvent OnDamageTaken;
        public UnityEvent OnHealed;

        /// <summary>
        /// Gets the current health value.
        /// </summary>
        public float CurrentHealth => _currentHealth;

        /// <summary>
        /// Gets the maximum health value.
        /// </summary>
        public float MaxHealth => _maxHealth;

        /// <summary>
        /// Gets or sets if the entity can take damage.
        /// </summary>
        public bool CanTakeDamage { get => _canTakeDamage; set => _canTakeDamage = value; }

        /// <summary>
        /// Gets or sets if the entity can be healed.
        /// </summary>
        public bool CanBeHealed { get => _canBeHealed; set => _canBeHealed = value; }

        /// <summary>
        /// Gets or sets if the entity can die.
        /// </summary>
        public bool CanDie { get => _canDie; set => _canDie = value; }

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// Reduces health by a given amount.
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (!_canTakeDamage || amount <= 0) return;

            _currentHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

            OnDamageTaken?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0 && _canDie)
                Die();
        }

        /// <summary>
        /// Restores health by a given amount.
        /// </summary>
        public void Heal(float amount)
        {
            if (!_canBeHealed || amount <= 0) return;

            _currentHealth += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

            OnHealed?.Invoke();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Fully restores health to max.
        /// </summary>
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Handles death behavior.
        /// </summary>
        private void Die()
        {
            OnDeath?.Invoke();

            // Optional: You could disable gameplay components here
            // Example: GetComponent<PlayerController>()?.enabled = false;
        }

        /// <summary>
        /// Sets a new maximum health and optionally resets current health.
        /// </summary>
        public void SetMaxHealth(float newMax, bool resetToFull = true)
        {
            _maxHealth = Mathf.Max(1f, newMax);
            if (resetToFull)
                _currentHealth = _maxHealth;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }
}
