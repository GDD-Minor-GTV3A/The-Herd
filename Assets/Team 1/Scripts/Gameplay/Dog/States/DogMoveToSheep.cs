using Core.AI.Sheep;
using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Dog moves to the target sheep.
    /// </summary>
    public class DogMoveToSheep : DogState
    {
        private SheepStateManager targetSheep;


        public DogMoveToSheep(DogStateManager manager) : base(manager)
        {
        }


        public override void OnStart()
        {
            manager.CurrentCommandTarget.OnValueChanged += OnTargetChanged;

            targetSheep = manager.HerdZone.GetFreeSheep();
        }

        public override void OnStop()
        {
            manager.CurrentCommandTarget.OnValueChanged -= OnTargetChanged;

            targetSheep = null;
        }

        public override void OnUpdate()
        {
            if (Vector3.Distance(manager.transform.position, targetSheep.transform.position) <= 3f)
            {
                manager.HerdZone.HeardSheep(targetSheep);

                manager.SetState<DogIdle>();

                return;
            }

            manager.MovementController.MoveTo(targetSheep.transform.position);
        }


        private void OnTargetChanged()
        {
            manager.SetState<DogMove>();
        }
    }
}