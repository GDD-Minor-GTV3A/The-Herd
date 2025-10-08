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
            UpdateRotationSpeed(rotationSpeed);
            _mainCamera = Camera.main;
        }


        /// <summary>
        /// Rotates the player to face the move forward direction.
        /// </summary>
        public void Rotate(Vector2 input)
        {

            //// Get camera forward vector ("beam" direction)
            //Vector3 camForward = _mainCamera.transform.forward;
            //camForward.y = 0f; // ignore vertical tilt

            //if (camForward.sqrMagnitude > 0.001f)
            //{
            //    Quaternion targetRotation = Quaternion.LookRotation(camForward);
            //    transform.rotation = Quaternion.Slerp(
            //        transform.rotation,
            //        targetRotation,
            //        _rotationSpeed * Time.deltaTime
            //    );
            //}

            Vector3 forward = _mainCamera.transform.forward;
            Vector3 right = _mainCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * input.y + right * input.x;

            if (move.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(move);
        }


        public void UpdateRotationSpeed(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }
    }
}
