using UnityEngine;
using System.Collections.Generic;
using Core.AI.Sheep;
using Gameplay.SheepEffects;
using Core.AI.Sheep.Config;

namespace Gameplay.Dog
{
    /// <summary>
    /// Handles logic of zone where player can add sheep to herd.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HerdZone : MonoBehaviour, ISheepEffectsEventsHandler
    {
        [Header("Herding Settings")]
        [SerializeField, Tooltip("Time when sheep can not be lost after adding to herd.")] 
        private float graceAfterJoin = 3f;

        [SerializeField, Tooltip("Tag which is used to find a sheep.")] 
        private string sheepTag = "Sheep";


        private List<SheepStateManager> freeSheep = new();
        private SphereCollider sphereCollider;

        PersonalityType ISheepEffectsEventsHandler.PersonalityType => PersonalityType.Nino;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            SheepEffectsDispatcher.AddNewListener(this);
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


        private void HeardSheep(SheepStateManager sheepToHeard)
        {
            if (freeSheep.Contains(sheepToHeard))
            {
                sheepToHeard.SummonToHerd(graceAfterJoin, clearThreats : true);
                sheepToHeard.SetDestinationWithHerding(transform.position);

                freeSheep.Remove(sheepToHeard);
            }
        }


        /// <summary>
        /// Adds all newarby free sheep to herd.
        /// </summary>
        public void HerdAllSheep()
        {
            if (freeSheep.Count == 0)
                return;

            foreach (SheepStateManager sheep in freeSheep)
            {
                HeardSheep(sheep);
            }
        }


        void ISheepEffectsEventsHandler.OnSheepJointHerd(SheepArchetype archetype)
        {
            // TO-DO: Decrease sphere collider radius on value from archetype
        }

        void ISheepEffectsEventsHandler.OnSheepLeftHerd(SheepArchetype archetype)
        {
            // TO-DO: Increase sphere collider radius on value from archetype
        }
    }
}