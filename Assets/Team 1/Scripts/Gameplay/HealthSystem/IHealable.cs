using UnityEngine.Events;

namespace Gameplay.HealthSystem
{
    public interface IHealable
    {
        public UnityEvent HealEvent { get; set; }


        public abstract void Heal(float amount);
    }
}
