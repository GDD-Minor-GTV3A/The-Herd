using UnityEngine.Events;

namespace Gameplay.HealthSystem
{
    public interface IKillable
    {
        public UnityEvent DeathEvent { get; set; }


        public abstract void Die();
    }
}
