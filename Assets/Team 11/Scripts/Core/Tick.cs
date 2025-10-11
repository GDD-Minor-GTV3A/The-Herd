using UnityEngine;
using UnityEngine.Events;

namespace Core.TickSystem
{
    public class Tick : MonoBehaviour
    {
        [SerializeField] private float tickInterval = 0.2f;

        private float tickTimer = 0f;

        public float TickDeltaTime => tickTimer;

        // Events
        public static UnityAction OnTickEvent;

        void Update()
        {
            tickTimer += Time.deltaTime;
            
            if (tickTimer >= tickInterval)
            {
                tickTimer = 0f;
                OnTickEvent?.Invoke();
            }
        }
    }
}
