using UnityEngine;
using System.Collections.Generic;
using Core.AI.Sheep;

namespace Gameplay.Dog
{
    /// <summary>
    /// Handles logic of zone where player can add sheep to herd.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HerdZone : MonoBehaviour
    {
        [Header("Herding Settings")]
        [SerializeField, Tooltip("Time when sheep can not be lost after adding to herd.")] 
        private float graceAfterJoin = 3f;

        [SerializeField, Tooltip("Tag which is used to find a sheep.")] 
        private string sheepTag = "Sheep";


        private List<SheepStateManager> freeSheep = new();


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            Collider _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            var _sheep = FindSheep(other);
            if (_sheep == null)
                return;

            if (_sheep.IsCurrentlyOutsideHerd())
            {
                freeSheep.Add(_sheep);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            var _sheep = FindSheep(other);
            if (_sheep == null)
                return;

            if (_sheep.IsCurrentlyOutsideHerd())
                freeSheep.Remove(_sheep);
        }


        private SheepStateManager FindSheep(Collider col)
        {
            if (!col.CompareTag(sheepTag))
                return null;

            return col.GetComponent<SheepStateManager>();
        }


        /// <summary>
        /// Returns first available sheep not in herd.
        /// </summary>
        /// <returns>First free sheep.</returns>
        public SheepStateManager GetFreeSheep()
        {
            if (freeSheep.Count == 0)
                return null;

            return freeSheep[0];
        }


        /// <summary>
        /// Adds sheep to herd.
        /// </summary>
        /// <param name="sheepToHeard">Sheep to add to herd.</param>
        public void HeardSheep(SheepStateManager sheepToHeard)
        {
            if (freeSheep.Contains(sheepToHeard))
            {
                sheepToHeard.SummonToHerd(graceAfterJoin, clearThreats : true);
                sheepToHeard.SetDestinationWithHerding(transform.position);

                freeSheep.Remove(sheepToHeard);
            }
        }


        /// <summary>
        /// Returns bool which says if there are any available free sheep.
        /// </summary>
        /// <returns>Are there any available sheep.</returns>
        public bool IsFreeSheepToHeard()
        {
            return freeSheep.Count != 0;
        }
    }
}