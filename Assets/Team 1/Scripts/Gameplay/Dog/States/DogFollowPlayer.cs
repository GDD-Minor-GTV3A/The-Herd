using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Dog is moving to the player.
    /// </summary>
    public class DogFollowPlayer : DogState
    {
        private readonly Transform playerTransform;
        private readonly float distanceToStopFollow;


        /// <param name="playerTransform">Transform of player object to follow.</param>
        /// <param name="distanceToStopFollow">Distance between dog and player to stop move.</param>
        public DogFollowPlayer(DogStateManager manager, Transform playerTransform, float distanceToStopFollow) : base(manager)
        {
            this.playerTransform = playerTransform;
            this.distanceToStopFollow = distanceToStopFollow;
        }


        public override void OnStart()
        {
            manager.CurrentCommandTarget.OnValueChanged += OnTargetChanged;
            animator.SetWalking(true);
        }

        public override void OnStop()
        {
            manager.CurrentCommandTarget.OnValueChanged -= OnTargetChanged;
        }

        public override void OnUpdate()
        {
            if (manager.HerdZone.IsFreeSheepToHeard())
                manager.SetState<DogMoveToSheep>();

            movement.MoveTo(CalculateFollowPoint());
            float _currentSpeed = movement.CalculateSpeedToPlayer();
            animator.CalculateWalkingSpeedMultiplier(_currentSpeed);

            if (!movement.IsMoving)
                manager.SetState<DogIdle>();
        }


        private Vector3 CalculateFollowPoint()
        {
            Vector3 _direction = (movement.transform.position - playerTransform.position).normalized;

            return playerTransform.position + _direction * distanceToStopFollow;
        }


        private void OnTargetChanged()
        {
            manager.SetState<DogMove>();
        }
    }
}