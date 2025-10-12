using UnityEngine;
using System.Collections.Generic;
using Core.AI.Sheep;

// Note: Uncomment the following lines to enable this functionality. This can only be done without compilation errors after the branch is merged with Team 2 latest version

namespace Gameplay.Dog
{
    [RequireComponent(typeof(Collider))]
    public class HeardZone : MonoBehaviour
    {
        [Header("Herding Settings")]
        [SerializeField] private bool _joinIfOutside = true;
        [SerializeField] private bool _oneJoiningPerSheep = true;
        [SerializeField] private float _graceAfterJoin = 3f;
        [SerializeField] private string _sheepTag = "Sheep";


        [SerializeField] private List<SheepStateManager> _freeSheep = new();


        public void Initialize()
        {
            Reset();
        }


        private void Reset()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var sheep = FindSheep(other);
            if (sheep == null)
                return;

            if (!_joinIfOutside || sheep.IsCurrentlyOutsideHerd())
            {
                _freeSheep.Add(sheep);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            var sheep = FindSheep(other);
            if (sheep == null)
                return;

            if (!_joinIfOutside || sheep.IsCurrentlyOutsideHerd())
                _freeSheep.Remove(sheep);
        }


        private SheepStateManager FindSheep(Collider col)
        {
            if (!col.CompareTag(_sheepTag))
                return null;

            return col.GetComponent<SheepStateManager>();
        }


        public SheepStateManager GetFreeSheep()
        {
            if (_freeSheep.Count == 0)
                return null;

            return _freeSheep[0];
        }


        public void HeardSheep(SheepStateManager sheepToHeard)
        {
            if (_freeSheep.Contains(sheepToHeard))
            {
                sheepToHeard.SummonToHerd();
                sheepToHeard.SetDestinationWithHerding(transform.position);

                _freeSheep.Remove(sheepToHeard);
            }
        }


        public bool IsFreeSheepToHeard()
        {
            return _freeSheep.Count != 0;
        }
    }
}
