using Core.Events;

namespace Gameplay.Dog
{
    /// <summary>
    /// Dog moves to the target of command.
    /// </summary>
    public class DogMove : DogState
    {
        public DogMove(DogStateManager manager) : base(manager)
        {
        }


        public override void OnStart()
        {
            UpdateTarget();

            animator.SetWalking(true);
            animator.SetWakingAnimationSpeedMulti(1f);

            manager.CurrentCommandTarget.OnValueChanged += UpdateTarget;

            EventManager.AddListener<DogFollowCommandEvent>(OnDogFollowCommand);
        }

        public override void OnStop()
        {
            manager.CurrentCommandTarget.OnValueChanged -= UpdateTarget;

            EventManager.RemoveListener<DogFollowCommandEvent>(OnDogFollowCommand);
        }

        public override void OnUpdate()
        {
            if (!movement.IsMoving)
            {
                manager.SetState<DogIdle>();
            }
        }


        private void UpdateTarget()
        {
            movement.MoveTo(manager.CurrentCommandTarget.Value);
        }


        private void OnDogFollowCommand(DogFollowCommandEvent evt)
        {
            manager.SetState<DogFollowPlayer>();
        }
    }
}