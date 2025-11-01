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
        [SerializeField] private bool joinIfOutside = true;
        [SerializeField] private float graceAfterJoin = 3f;
        [SerializeField] private string sheepTag = "Sheep";


        private List<SheepStateManager> freeSheep = new();


        public void Initialize()
        {
            Reset();
        }


        private void Reset()
        {
            var _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var _sheep = FindSheep(other);
            if (_sheep == null)
                return;

            if (!joinIfOutside || _sheep.IsCurrentlyOutsideHerd())
            {
                freeSheep.Add(_sheep);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            var _sheep = FindSheep(other);
            if (_sheep == null)
                return;

            if (!joinIfOutside || _sheep.IsCurrentlyOutsideHerd())
                freeSheep.Remove(_sheep);
        }


        private SheepStateManager FindSheep(Collider col)
        {
            if (!col.CompareTag(sheepTag))
                return null;

            return col.GetComponent<SheepStateManager>();
        }


        public SheepStateManager GetFreeSheep()
        {
            if (freeSheep.Count == 0)
                return null;

            return freeSheep[0];
        }


        public void HeardSheep(SheepStateManager sheepToHeard)
        {
            if (freeSheep.Contains(sheepToHeard))
            {
                sheepToHeard.SummonToHerd(graceAfterJoin, clearThreats : true);
                sheepToHeard.SetDestinationWithHerding(transform.position);

                freeSheep.Remove(sheepToHeard);
            }
        }


        public bool IsFreeSheepToHeard()
        {
            return freeSheep.Count != 0;
        }
    }
}