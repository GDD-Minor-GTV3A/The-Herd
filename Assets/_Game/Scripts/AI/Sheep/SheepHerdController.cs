using UnityEngine;
using Core.Events;
using System.Collections;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Tracking player's square and raising event from manager
    /// </summary>
    public partial class SheepHerdController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Player's square Transform")]
        private Transform _player;

        [SerializeField]
        [Tooltip("Half size of player's square XZ")]
        private Vector2 _halfExtents = new Vector2(4f, 4f);

        [SerializeField]
        [Tooltip("Broadcasting sheep tick event")]
        private float _interval = 0.2f;


        private Coroutine _tickCoroutine;


        private void OnEnable()
        {
            EventManager.Broadcast(new PlayerSquareChangedEvent(_player.position, _halfExtents));
            _tickCoroutine = StartCoroutine(Tick());
        }

        private void OnDisable()
        {
            if(_tickCoroutine != null )
            {
                StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;
            }
        }


        private IEnumerator Tick()
        {
            var wait = new WaitForSeconds(_interval);
            while (true)
            {
                EventManager.Broadcast(new PlayerSquareTickEvent(_player.position, _halfExtents));
                yield return wait;
            }
        }

        /// <summary>
        /// Adjust the square at runtime
        /// </summary>
        public void SetHalfExtents(Vector2 halfExtents)
        {
            _halfExtents = halfExtents;
            EventManager.Broadcast(new PlayerSquareChangedEvent(_player.position, _halfExtents));
        }
    }

}
