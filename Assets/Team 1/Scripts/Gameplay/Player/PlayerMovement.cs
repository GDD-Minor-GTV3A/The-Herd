using Core.Shared;

using Gameplay.Dog;

using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Controls player movement.
    /// </summary>
    public class PlayerMovement : MovementController
    {
        private CharacterController _controller;
        private Camera _camera;

        private float _walkSpeed;
        private float _runSpeed;
        private float _gravity;

        private float _rotationSpeed;

        private float _verticalVelocity = 0f;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="characterController">Character controller of the player. Used for movement.</param>
        /// <param name="config">Config of the player.</param>
        public void Initialize(CharacterController characterController, PlayerConfig config)
        {
            _camera = Camera.main;
            _controller = characterController;

            UpdateValues(config);
        }


        public override void MoveTo(Vector3 target)
        {
            _controller.Move(target);
        }


        public void Rotate(Vector3 input)
        {
            if (input.sqrMagnitude < 0.0001f)
                return;

            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * input.y + right * input.x;

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }


        /// <summary>
        /// Applies gravity to the player.
        /// </summary>
        public void ApplyGravity()
        {
            // Apply gravity
            if (_controller.isGrounded)
            {
                _verticalVelocity = -1f; // keep grounded
            }
            else
            {
                _verticalVelocity += _gravity;
            }

            _controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
        }


        /// <summary>
        /// Calculates where player should be moved according to input.
        /// </summary>
        /// <param name="moveInput">Input of the player.</param>
        /// <param name="isRunning">Is player holding sprint button.</param>
        /// <returns>Final destination of the player.</returns>
        public Vector3 CalculateMovementTargetFromInput(Vector2 moveInput, bool isRunning)
        {
            // Convert input to world space relative to camera
            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * moveInput.y + right * moveInput.x;

            if (move.magnitude > 1f)
                move.Normalize();

            float speed = isRunning ? _runSpeed : _walkSpeed;


            return move * speed * Time.deltaTime;
        }


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the player.</param>
        public void UpdateValues(PlayerConfig config)
        {
            _walkSpeed = config.WalkSpeed;
            _runSpeed = config.RunSpeed;
            _gravity = config.Gravity;
            _rotationSpeed = config.RotationSpeed;
        }
    }
}