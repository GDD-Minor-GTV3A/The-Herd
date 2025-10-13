using System;

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
        public event Action<float, float> OnHealthChanged; // (current, max)


        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }
        public bool CanBeHealed { get => canBeHealed; set => canBeHealed = value; }
        public bool CanDie { get => canDie; set => canDie = value; }


        public Health(float maxHealth = 100f, float currentHealth = 100f, bool canTakeDamage = true, bool canBeHealed = true, bool canDie = true)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = this.maxHealth;
            this.canTakeDamage = canTakeDamage;
            this.canBeHealed = canBeHealed;
            this.canDie = canDie;
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


        public void ChangeCurrentHealth(float value)
        {
            currentHealth += value;
            currentHealth = Math.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}