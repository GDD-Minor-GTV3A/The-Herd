using UnityEngine.Events;
using UnityEngine;

namespace Gameplay.HealthSystem
{
    /// <summary>
    /// Makes entity damagaeble.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Invokes when entity gets damage.
        /// </summary>
        public UnityEvent DamageEvent { get; set; }


        /// <summary>
        /// Dealing damage to entity.
        /// </summary>
        /// <param name="damage">amount of damage.</param>
        public abstract void TakeDamage(float damage);
    }
}