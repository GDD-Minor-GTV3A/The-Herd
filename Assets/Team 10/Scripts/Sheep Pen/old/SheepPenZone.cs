using UnityEngine;
using Core.AI.Sheep;

namespace Gameplay.Pen
{
    /// <summary>
    /// Controls the sheep pen zone behavior.
    /// When the player enters the zone, all active sheep either
    /// follow the decoy inside the pen or return to following the player.
    /// </summary>
    public class SheepPenZone : MonoBehaviour
    {
        [Header("References")]

        [SerializeField]
        [Tooltip("The decoy Transform inside the pen that sheep should follow.")]
        private Transform _decoyTransform;

        [SerializeField]
        [Tooltip("Reference to the player Transform.")]
        private Transform _playerTransform;

        private bool _sheepFollowingDecoy = false;

        private void Start()
        {
            // Ensure sheep begin following the player at game start
            SheepStateManager[] sheepManagers = FindObjectsOfType<SheepStateManager>();

            foreach (var sheep in sheepManagers)
            {
                if (sheep == null)
                    continue;

                sheep.OnRejoinedHerd();
            }

            Debug.Log("Sheep initialized to follow player normally.");
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _sheepFollowingDecoy = !_sheepFollowingDecoy;

            if (_sheepFollowingDecoy)
            {
                SendSheepToDecoy();
                Debug.Log("Sheep are now following the DECOY (pen).");
            }
            else
            {
                ReturnSheepToPlayer();
                Debug.Log("Sheep are now following the PLAYER.");
            }
        }
    
        /// Sends all active sheep to the decoy's position inside the pen.
   
        private void SendSheepToDecoy()
        {
            SheepStateManager[] sheepManagers = FindObjectsOfType<SheepStateManager>();

            foreach (var sheep in sheepManagers)
            {
                if (sheep == null || !sheep.CanControlAgent())
                    continue;

                // Temporarily disable AI logic so it does not override movement
                sheep.enabled = false;

                Vector2 offset = Random.insideUnitCircle * 1.5f;
                Vector3 destination = _decoyTransform.position + new Vector3(offset.x, 0f, offset.y);
                sheep.Agent.SetDestination(destination);
            }
        }


        /// Reactivates sheep AI and makes them follow the player again.
        private void ReturnSheepToPlayer()
        {
            SheepStateManager[] sheepManagers = FindObjectsOfType<SheepStateManager>();

            foreach (var sheep in sheepManagers)
            {
                if (sheep == null)
                    continue;

                sheep.enabled = true;

                if (sheep.CanControlAgent())
                {
                    Vector2 offset = Random.insideUnitCircle * 1.5f;
                    Vector3 destination = _playerTransform.position + new Vector3(offset.x, 0f, offset.y);
                    sheep.Agent.SetDestination(destination);
                }

                sheep.OnRejoinedHerd();
            }
        }
    }
}
