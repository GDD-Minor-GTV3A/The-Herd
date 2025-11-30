using Core.AI.Sheep.Config;
using Core.Shared;
using Gameplay.SheepEffects;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Controls player movement.
    /// </summary>
    public class PlayerMovement : MovementController, ISheepEffectsEventsHandler
    {
        private CharacterController controller;
        private Camera mainCamera;

        private float walkSpeed;
        private float runSpeed;
        private float gravity;
        private float speedModifier = 0f;

        private float rotationSpeed;

        private float verticalVelocity = 0f;

        PersonalityType ISheepEffectsEventsHandler.PersonalityType => PersonalityType.Ivana;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="characterController">Character controller of the player. Used for movement.</param>
        /// <param name="config">Config of the player.</param>
        public void Initialize(CharacterController characterController, PlayerConfig config)
        {
            mainCamera = Camera.main;
            controller = characterController;

            UpdateValues(config);

            SheepEffectsDispatcher.AddNewListener(this);
        }


        public override void MoveTo(Vector3 target)
        {
            controller.Move(target);
        }


        public void Rotate(Vector3 input)
        {
            if (input.sqrMagnitude < 0.0001f)
                return;
            
            Vector3 _move = GetMoveDirection(input);

            Quaternion _targetRotation = Quaternion.LookRotation(_move);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }


        /// <summary>
        /// Applies gravity to the player.
        /// </summary>
        public void ApplyGravity()
        {
            // Apply gravity
            if (controller.isGrounded)
            {
                verticalVelocity = -1f; // keep grounded
            }
            else
            {
                verticalVelocity += gravity;
            }

            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }


        /// <summary>
        /// Calculates where player should be moved according to input.
        /// </summary>
        /// <param name="moveInput">Input of the player.</param>
        /// <param name="isRunning">Is player holding sprint button.</param>
        /// <returns>Final destination of the player.</returns>
        public Vector3 CalculateMovementTargetFromInput(Vector2 moveInput, bool isRunning)
        {
            Vector3 _move = GetMoveDirection(moveInput);

            if (_move.magnitude > 1f)
                _move.Normalize();

            float _speed = isRunning ? runSpeed : walkSpeed;


            return _move * ((_speed + speedModifier) * Time.deltaTime);
        }


        private Vector3 GetMoveDirection(Vector2 moveInput)
        {
            // Convert input to world space relative to camera

            Vector3 _forward = mainCamera.transform.forward;
            Vector3 _right = mainCamera.transform.right;

            _forward.y = 0f;
            _right.y = 0f;

            _forward.Normalize();
            _right.Normalize();

            Vector3 _move = _forward * moveInput.y + _right * moveInput.x;

            return _move;
        }


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the player.</param>
        public void UpdateValues(PlayerConfig config)
        {
            walkSpeed = config.WalkSpeed;
            runSpeed = config.RunSpeed;
            gravity = config.Gravity;
            rotationSpeed = config.RotationSpeed;
        }


        void ISheepEffectsEventsHandler.OnSheepJointHerd(SheepArchetype archetype)
        {
            // TO-DO: invoke UpdateSpeedModifier with value of speed change
        }

        void ISheepEffectsEventsHandler.OnSheepLeftHerd(SheepArchetype archetype)
        {
            // TO-DO: invoke UpdateSpeedModifier with negative value of speed change
        }
    }
}