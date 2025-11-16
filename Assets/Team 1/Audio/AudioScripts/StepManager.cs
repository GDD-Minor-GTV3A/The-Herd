using UnityEngine;

public class StepManager : MonoBehaviour
{
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
                    AudioClip clip = set.footsteps[Random.Range(0, set.footsteps.Length)];
                    audioSource.PlayOneShot(clip);
                }
                return;
            }
        }
    }
}
