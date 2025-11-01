using Core.Shared;

using Gameplay.ToolsSystem;

using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Handles barking logic, detecting scareable entities and applying scare effects.
    /// </summary>
    public class DogBark : MonoBehaviour
    {
        [Header("Bark Settings")]
        [Tooltip("Maximum distance the bark can reach.")]
        [SerializeField] private float _maxBarkDistance = 10f;

        [Tooltip("Angle (in degrees) of the bark cone in front of the dog.")]
        [SerializeField] private float _barkAngle = 90f;

        [Tooltip("Cooldown between barks (in seconds).")]
        [SerializeField] private float _barkCooldown = 2f;

        [Tooltip("Physics layers to detect scareable objects.")]
        [SerializeField] private LayerMask _scareableMask;

        [Tooltip("Optional animator to trigger bark animation.")]
        [SerializeField] private Animator _animator;

        private float _lastBarkTime;

        /// <summary>
        /// Attempts to bark. Will respect cooldown timer.
        /// </summary>
        public void TryBark()
        {

            if (Time.time - _lastBarkTime < _barkCooldown)
                return;

            _lastBarkTime = Time.time;
            Bark();
        }

        /// <summary>
        /// Performs the bark detection and triggers scareable objects.
        /// </summary>
        private void Bark()
        {
            Debug.Log("Bark!!!");
            // Trigger bark animation or sound if available                                                                             
            if (_animator != null)
                _animator.SetTrigger("Bark");

            // Collect all potential targets within max range
            Collider[] hits = Physics.OverlapSphere(transform.position, _maxBarkDistance, _scareableMask);

            foreach (Collider hit in hits)
            {
                Vector3 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, toTarget);

                // Check if target is within the forward cone
                if (angle <= _barkAngle * 0.5f)
                {
                    if (hit.TryGetComponent(out IScareable scareable))
                    {
                        float distance = Vector3.Distance(transform.position, hit.transform.position);
                        float intensity = Mathf.Clamp01(1f - (distance / _maxBarkDistance));

                        // Pass scare type info here
                        scareable.OnScared(transform.position, intensity, ScareType.DogBark);
                    }
                }
            }

            // Optional: visualize the bark cone for debugging
            DebugDrawBarkZone();
        }

        /// <summary>
        /// Draws a visual representation of the bark range for debugging.
        /// </summary>
        private void DebugDrawBarkZone()
        {
            Vector3 leftLimit = Quaternion.Euler(0, -_barkAngle * 0.5f, 0) * transform.forward;
            Vector3 rightLimit = Quaternion.Euler(0, _barkAngle * 0.5f, 0) * transform.forward;

            Debug.DrawLine(transform.position, transform.position + leftLimit * _maxBarkDistance, Color.yellow, 1f);
            Debug.DrawLine(transform.position, transform.position + rightLimit * _maxBarkDistance, Color.yellow, 1f);
            Debug.DrawLine(transform.position, transform.position + transform.forward * _maxBarkDistance, Color.cyan, 1f);
        }
    }
}
