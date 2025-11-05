using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles player rotation logic, supporting both movement and mouse-based rotation modes.
    /// </summary>
    public class PlayerRotation : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 10f;

        [Tooltip("Select which rotation mode the player starts in.")]
        [SerializeField] private PlayerRotationMode _startingRotationMode = PlayerRotationMode.MovementDirection;


        private Camera _mainCamera;
        private PlayerRotationMode _rotationMode;

        public PlayerRotationMode RotationMode => _rotationMode;


        /// <summary>
        /// Sets the rotation speed for smoothing the player's turn.
        /// </summary>
        public void Initialize(float rotationSpeed)
        {
            _mainCamera = Camera.main;

            SetRotationMode(_startingRotationMode);

            UpdateRotationSpeed(rotationSpeed);
        }


        public void UpdateRotationSpeed(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }


        /// <summary>
        /// Switches between movement-based and mouse-based rotation.
        /// </summary>
        public void ToggleRotationMode()
        {
            _rotationMode = _rotationMode == PlayerRotationMode.MovementDirection
                ? PlayerRotationMode.MouseDirection
                : PlayerRotationMode.MovementDirection;
        }


        /// <summary>
        /// Sets rotation mode directly.
        /// </summary>
        public void SetRotationMode(PlayerRotationMode mode)
        {
            _rotationMode = mode;
        }


        /// <summary>
        /// Rotates the player based on the current rotation mode.
        /// </summary>
        public void Rotate(Vector2 moveInput, Vector3 mouseWorldPosition)
        {
            switch (_rotationMode)
            {
                case PlayerRotationMode.MovementDirection:
                    RotateByMovement(moveInput);
                    break;

                case PlayerRotationMode.MouseDirection:
                    RotateByMouseWorld(mouseWorldPosition);
                    break;
            }
        }


        private void RotateByMovement(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f)
                return;

            Vector3 forward = _mainCamera.transform.forward;
            Vector3 right = _mainCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * input.y + right * input.x;

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private void RotateByMouseWorld(Vector3 worldPosition)
        {
            Vector3 direction = worldPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        }
    }


    /// <summary>
    /// The available player rotation modes.
    /// </summary>
    public enum PlayerRotationMode
    {
        MovementDirection,
        MouseDirection
    }
}
