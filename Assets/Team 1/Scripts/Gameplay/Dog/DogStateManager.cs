using System.Collections.Generic;

using Core.Events;
using Core.Shared;
using Core.Shared.StateMachine;

using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// State machine for the dog.
    /// </summary>
    public class DogStateManager : CharacterStateManager<DogState>
    {
        private Transform _playerTransform;
        private float _distanceToPlayer;


        /// <summary>
        /// Target of dog's command. CANNOT be the player.
        /// </summary>
        public Observable<Vector3> CurrentTarget { get; set; } = new Observable<Vector3>();


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="movementController">Dog movement controller.</param>
        /// <param name="animator">Dog animator controller.</param>
        /// <param name="playerTransform">Player transform to follow.</param>
        /// <param name="config">Config of the dog.</param>
        public void Initialize(DogMovementController movementController, DogAnimator animator, Transform playerTransform, DogConfig config)
        {
            _movementController = movementController;

            _animatorController = animator;

            _playerTransform = playerTransform;

            _distanceToPlayer = config.DistanceToPlayer;

            InitializeStatesMap();

            EventManager.AddListener<DogMoveCommandEvent>(OnDogMoveCommand);

            SetState<DogIdle>();
        }


        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<System.Type, DogState>
            {
                { typeof(DogIdle), new DogIdle(this, _playerTransform, _distanceToPlayer) },
                { typeof(DogFollowPlayer), new DogFollowPlayer(this, _playerTransform, _distanceToPlayer) },
                { typeof(DogMove), new DogMove(this) },
            };
        }


        private void OnDogMoveCommand(DogMoveCommandEvent evt)
        {
            CurrentTarget.Value = evt.MoveTarget;
        }
    }
}