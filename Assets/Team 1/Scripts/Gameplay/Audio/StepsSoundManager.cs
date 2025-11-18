using UnityEngine;

namespace Gameplay.Audio 
{
    /// <summary>
    /// Manages sounds of steps for entity.
    /// </summary>
    public class StepsSoundManager : MonoBehaviour
    {
        /// <summary>
        /// Contains pairs of surface type and audio clipds for them.
        /// </summary>
        [System.Serializable]
        public class FootstepSet
        {
            public string surfaceTag;
            public AudioClip[] footsteps;
        }

        public FootstepSet[] footstepSets;
        public AudioSource audioSource;

        private SurfaceDetector detector;

        private void Awake()
        {
            detector = GetComponent<SurfaceDetector>();
        }

        public void PlayFootstep()
        {
            var surface = detector.CurrentSurface;

            foreach (var set in footstepSets)
            {
                if (set.surfaceTag == surface)
                {
                    if (set.footsteps.Length > 0)
                    {
                        AudioClip _clip = set.footsteps[Random.Range(0, set.footsteps.Length)];
                        audioSource.PlayOneShot(_clip);
                    }
                    return;
                }
            }
        }
    }

}