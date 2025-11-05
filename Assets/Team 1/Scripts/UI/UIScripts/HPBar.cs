using Gameplay.HealthSystem;

using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    private Health health;

    void UpdateHPSlider(float CurrentHP, float MaxHP)
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
