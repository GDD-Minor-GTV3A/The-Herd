using UnityEngine.Events;

namespace Gameplay.HealthSystem
{
    public interface IDamageable
    {
        public UnityEvent DamageEvent { get; set; }


        public abstract void TakeDamage(float damage);
    }
}
