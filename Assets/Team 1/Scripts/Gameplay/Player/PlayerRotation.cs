using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles player rotation logic.
    /// </summary>
    public class PlayerRotation : MonoBehaviour
    {
        private float _rotationSpeed;
        private Camera _mainCamera;

        /// <summary>
        /// Sets the rotation speed for smoothing the player's turn.
        /// </summary>
        /// <param name="rotationSpeed">How quickly the player rotates towards the camera's direction.</param>
        public void Initialize(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }

        private void Awake()
        {
            // Cache reference to main camera
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Rotates the player to face the camera's forward direction.
        /// </summary>
        public void RotateToCamera()
        {
            if (_mainCamera == null) return;

            // Get camera forward vector ("beam" direction)
            Vector3 camForward = _mainCamera.transform.forward;
            camForward.y = 0f; // ignore vertical tilt

            if (camForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(camForward);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
