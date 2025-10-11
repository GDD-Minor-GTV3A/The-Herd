using System;

namespace Gameplay.HealthSystem
{
    /// <summary>
    /// Interface for a universal health system.
    /// Allows external scripts to interact with any class that supports health operations.
    /// </summary>
    public interface IHealth
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }

        bool CanTakeDamage { get; set; }
        bool CanBeHealed { get; set; }
        bool CanDie { get; set; }

        void TakeDamage(float amount);
        void Heal(float amount);
        void ResetHealth();
        void SetMaxHealth(float newMax, bool resetToFull = true);

        event Action<float, float> OnHealthChanged;
        event Action OnDeath;
        event Action OnDamageTaken;
        event Action OnHealed;
    }
}
