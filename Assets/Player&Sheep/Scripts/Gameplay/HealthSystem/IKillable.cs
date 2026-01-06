using UnityEngine.Events;

namespace Gameplay.HealthSystem
{
    /// <summary>
    /// Makes entity killable.
    /// </summary>
    public interface IKillable
    {
        /// <summary>
        /// Invokes when entity dies.
        /// </summary>
        public UnityEvent DeathEvent { get; set; }


        /// <summary>
        /// Kills entity.
        /// </summary>
        public abstract void Die();
    }
}
