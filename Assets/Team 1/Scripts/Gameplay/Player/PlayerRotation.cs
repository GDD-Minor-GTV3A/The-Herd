using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles player rotation logic.
    /// </summary>
    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        private Camera _mainCamera;
        [SerializeField] private LayerMask _groundMask = ~0; // Default: everything

        /// <summary>
        /// Sets the rotation speed for smoothing the player's turn.
        /// </summary>
        public void Initialize(float rotationSpeed)
        {
            UpdateRotationSpeed(rotationSpeed);
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Rotates the player toward the direction where the camera center hits the ground.
        /// </summary>
        public void Rotate(Vector2 input)
        {
            if (_mainCamera == null)
                return;

            // Step 1: Cast a ray from camera center
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.8f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, _groundMask))
            {
                Vector3 lookPoint = hit.point;

                // Step 2: Find direction from player to that point (ignore height)
                Vector3 lookDir = lookPoint - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.001f)
                {
                    // Step 3: Smoothly rotate toward it
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        _rotationSpeed * Time.deltaTime
                    );
                }
            }
        }

        public void UpdateRotationSpeed(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }
    }
}
