using Core.AI.Sheep;
using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Dog moves to the target sheep.
    /// </summary>
    public class TryToHerdSheep : DogState
    {
        public TryToHerdSheep(DogStateManager manager) : base(manager)
        {
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            Debug.Log("Herding all sheep");
            manager.HerdZone.herdAllSheep();
            manager.SetState<DogIdle>();
        }
    }
}