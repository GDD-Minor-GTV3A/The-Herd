using UnityEngine;

namespace Gameplay.Audio
{
    public class SurfaceDetector : MonoBehaviour
    {

        [SerializeField] private LayerMask groundMask;
        public string CurrentSurface { get; private set; }

        private void Update()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, groundMask))
            {
                CurrentSurface = hit.collider.tag;
            }
            else
            {
                CurrentSurface = "Default";
            }
        }
    }
}