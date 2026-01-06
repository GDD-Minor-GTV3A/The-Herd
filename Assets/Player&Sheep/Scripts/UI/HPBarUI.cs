using Core.Shared.Utilities;
using Gameplay.HealthSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Handles logic of hp bar.
    /// </summary>
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField, Tooltip("Slider which controls hp amount visualization"), Required]
        private Slider slider;


        private Health health;


        private void UpdateHPSlider(float CurrentHP, float MaxHP)
        {
            slider.value = CurrentHP / MaxHP;
        }


        public void Initialize(Health health)
        {
            this.health = health;
            this.health.OnHealthChanged += UpdateHPSlider;
        }


        private void OnDestroy()
        {
            this.health.OnHealthChanged -= UpdateHPSlider;
        }
    }
}