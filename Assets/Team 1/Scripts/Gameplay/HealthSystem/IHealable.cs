using UnityEngine.Events;

namespace Gameplay.HealthSystem
{
    /// <summary>
    /// Makes entity healable.
    /// </summary>
    public interface IHealable
    {
        /// <summary>
        /// Invokes when entity gets healed.
        /// </summary>
        public UnityEvent HealEvent { get; set; }



        /// <summary>
        /// Heals entity.
        /// </summary>
        /// <param name="amount">Amount of heal.</param>
        public abstract void Heal(float amount);
    }
}
