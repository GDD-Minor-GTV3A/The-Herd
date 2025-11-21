using Core.Shared.StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player 
{
    /// <summary>
    /// Player state machine.
    /// </summary>
    public class PlayerStateManager : CharacterStateManager<PlayerState>
    {
        /// <summary>
        /// Player input.
        /// </summary>
        public PlayerInput Input { get; private set; }


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="input">Player input class.</param>
        /// <param name="movement">Player movement class.</param>
        /// <param name="playerRotation">Player rotation class.</param>
        public void Initialize(PlayerInput input, PlayerMovement movement, PlayerAnimator animator)
        {
            Input = input;
            _movementController = movement;
            _animatorController = animator;

            InitializeStatesMap();
            SetState<PlayerIdle>();
        }


        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<System.Type, PlayerState>
            {
                { typeof(PlayerIdle), new PlayerIdle(this) },
                { typeof(PlayerWalking), new PlayerWalking(this) },
                { typeof(PlayerRunning), new PlayerRunning(this) },
            };
        }
    }
}