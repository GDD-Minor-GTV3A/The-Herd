using Core.AI.Sheep;

using UnityEngine;

namespace Gameplay.Dog
{
    public class DogMoveToSheep : DogState
    {
        private SheepStateManager _targetSheep;


        public DogMoveToSheep(DogStateManager manager) : base(manager)
        {
        }

        public override void OnStart()
        {
            _manager.CurrentCommandTarget.OnValueChanged += OnTargetChanged;


            _targetSheep = _manager.HeardZone.GetFreeSheep();
        }

        public override void OnStop()
        {
            _manager.CurrentCommandTarget.OnValueChanged -= OnTargetChanged;

            _targetSheep = null;
        }

        public override void OnUpdate()
        {
            if (Vector3.Distance(_manager.transform.position, _targetSheep.transform.position) <= 3f)
            {
                _manager.HeardZone.HeardSheep(_targetSheep);

                _manager.SetState<DogIdle>();

                return;
            }


            _manager.MovementController.MoveTo(_targetSheep.transform.position);
        }


        private void OnTargetChanged()
        {
            _manager.SetState<DogMove>();
        }
    }
}
