using UnityEngine;
using UnityEngine.Events;

namespace Core.TickSystem
{
    public class Tick : MonoBehaviour
    {
        [Tooltip("Interval in seconds between each tick.")]
        [SerializeField] private float tickInterval = 0.2f;

        private float tickTimer = 0f;

        /// <summary>
        /// Event triggered on each tick interval. Subscribers can listen to this event to perform actions at regular intervals.
        /// </summary>
        public static UnityAction OnTickAction;

        void Update()
        {
            tickTimer += Time.deltaTime;
            
            if (tickTimer >= tickInterval)
            {
                tickTimer = 0f;
                OnTickAction?.Invoke();
            }
        }
    }
}
