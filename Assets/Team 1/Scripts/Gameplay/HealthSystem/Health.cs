using System;

using Gameplay.Player;

namespace Gameplay.HealthSystem
{
    /// <summary>
    /// A universal health system for any entity.
    /// No longer a MonoBehaviour — just a pure class.
    /// </summary>
    public class Health
    {
        private float maxHealth;
        private float currentHealth;
        private bool canTakeDamage;
        private bool canBeHealed;
        private bool canDie;


        // Events (replace UnityEvent with C# events)
        /// <summary>
        /// Invokes when health changed.
        /// </summary>
        public event Action<float, float> OnHealthChanged; // (current, max)


        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }
        public bool CanBeHealed { get => canBeHealed; set => canBeHealed = value; }
        public bool CanDie { get => canDie; set => canDie = value; }


        public Health(PlayerConfig config)
        {
            UpdateValuesFromConfig(config);
        }


        /// <summary>
        /// Set current hp to max hp.
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }


        /// <summary>
        /// Changes max hp.
        /// </summary>
        /// <param name="newMax">New value of max hp.</param>
        /// <param name="resetToFull">If needed to reset to current hp to max.</param>
        public void SetMaxHealth(float newMax, bool resetToFull = true)
        {
            maxHealth = Math.Max(1f, newMax);
            if (resetToFull)
                currentHealth = maxHealth;

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }


        /// <summary>
        /// For dealing damage of healing.
        /// </summary>
        /// <param name="value"></param>
        public void ChangeCurrentHealth(float value)
        {
            currentHealth += value;
            currentHealth = Math.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }


        public void UpdateValuesFromConfig(PlayerConfig config)
        {
            maxHealth = config.MaxHealth;
            currentHealth = config.CurrentHealth;
            canTakeDamage = config.CanTakeDamage;
            canDie = config.CanDie;
            canBeHealed = config.CanBeHealed;
        }
    }
}