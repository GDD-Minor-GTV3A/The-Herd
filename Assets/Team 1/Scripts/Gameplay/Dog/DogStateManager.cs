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
        private Transform playerTransform;
        private float distanceToPlayer;
        private HerdZone herdZone;


        /// <summary>
        /// Target of dog's command. CANNOT be the player.
        /// </summary>
        public Observable<Vector3> CurrentCommandTarget { get; set; } = new Observable<Vector3>();
        /// <summary>
        /// Reference to the herd controller.
        /// </summary>
        public HerdZone HerdZone => herdZone;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="movementController">Dog movement controller.</param>
        /// <param name="animator">Dog animator controller.</param>
        /// <param name="playerTransform">Player transform to follow.</param>
        /// <param name="config">Config of the dog.</param>
        public void Initialize(DogMovementController movementController, DogAnimator animator, HerdZone heardZone, Transform playerTransform, DogConfig config)
        {
            _movementController = movementController;

            this.herdZone = heardZone;

            _animatorController = animator;

            this.playerTransform = playerTransform;

            distanceToPlayer = config.DistanceToPlayer;

            InitializeStatesMap();

            EventManager.AddListener<DogMoveCommandEvent>(OnDogMoveCommand);

            SetState<DogIdle>();
        }


        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<System.Type, DogState>
            {
                { typeof(DogIdle), new DogIdle(this, playerTransform, distanceToPlayer) },
                { typeof(DogFollowPlayer), new DogFollowPlayer(this, playerTransform, distanceToPlayer) },
                { typeof(DogMove), new DogMove(this) },
            };
        }


        private void OnDogMoveCommand(DogMoveCommandEvent evt)
        {
            CurrentCommandTarget.Value = evt.MoveTarget;
        }
    }
}