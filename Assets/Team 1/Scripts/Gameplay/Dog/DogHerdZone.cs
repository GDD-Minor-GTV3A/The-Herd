using UnityEngine;
using System.Collections.Generic;

// Note: Uncomment the following lines to enable this functionality. This can only be done without compilation errors after the branch is merged with Team 2 latest version

namespace Gameplay.Dog
{
    [RequireComponent(typeof(Collider))]
    public class DogHerdZone : MonoBehaviour
    {
        [Header("Herding Settings")]
        [SerializeField] private bool _joinIfOutside = true;
        [SerializeField] private bool _oneJoiningPerSheep = true;
        [SerializeField] private float _graceAfterJoin = 3f;
        [SerializeField] private string _sheepTag = "Sheep";

        //private readonly HashSet<Core.AI.Sheep.SheepStateManager> _joinedSheep = new();

        // Herding toggle state
        private bool _herdingActive = true;

        /// <summary>
        /// Toggle herd mode on or off
        /// </summary>
        public void ToggleHerding()
        {
            _herdingActive = !_herdingActive;
            Debug.Log($"Herding is now {(_herdingActive ? "ON" : "OFF")}");
        }

        /// <summary>
        /// Expose current state
        /// </summary>
        public bool IsHerdingActive() => _herdingActive;

        //private void Reset()
        //{
        //    var collider = GetComponent<Collider>();
        //    collider.isTrigger = true;
        //}

        //private void OnTriggerEnter(Collider other)
        //{

        //    if (!_herdingActive)
        //        return;

        //    var sheep = FindSheep(other);
        //    if (sheep == null)
        //        return;

        //    // Skip if already processed and one-per-sheep is enabled

        //    if (_oneJoiningPerSheep && _joinedSheep.Contains(sheep))
        //        return;

        //    // Check if sheep should join

        //    if (!_joinIfOutside || sheep.IsCurrentlyOutsideHerd())
        //    {
        //        // Summon sheep to herd

        //        sheep.SummonToHerd(_graceAfterJoin, clearThreats: true);

        //        // Immediately override herd target to dog's position

        //        sheep.SetDestinationWithHerding(transform.position);

        //        //_joinedSheep.Add(sheep);
        //    }
        //}

        //private Core.AI.Sheep.SheepStateManager FindSheep(Collider col)
        //{
        //    if (!col.CompareTag(_sheepTag))
        //        return null;

        //    return col.GetComponent<Core.AI.Sheep.SheepStateManager>();
        //}

        //public void ClearTrackedSheep()
        //{
        //    _joinedSheep.Clear();
        //}
    }
}
