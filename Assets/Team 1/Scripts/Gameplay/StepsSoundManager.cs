using System.Collections.Generic;
using Core.Shared.Utilities;
using UnityEngine;

using static GroundCheck;

namespace Gameplay
{
    /// <summary>
    /// Handles playing steps sound.
    /// </summary>
    public class StepsSoundManager : MonoBehaviour
    {
        ///Use SerializeField instead of public(public == bad)
        ///Use Tooltips for SerializeFiled
        [SerializeField, Tooltip("Sound clips for steps sounds.")]
        private List<AudioClip> snowFS; /// for private use camelCase naming, for public PascalCase
        
        [SerializeField, Tooltip("Sound clips for steps sounds.")]
        private List<AudioClip> dirtFS;
        
        [SerializeField, Tooltip("Sound clips for steps sounds.")]
        private List<AudioClip> rockFS;
        
        [SerializeField, Tooltip("Sound clips for steps sounds.")]
        private List<AudioClip> iceFS;

        [SerializeField, Tooltip("Sound source for steps."), Required]
        private AudioSource footstepSource;

        [SerializeField, Required] 
        private GroundCheck groundCheck;

        
        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize() /// we dont use Start() and Awake(), instead use Initialize()
        {
            if (footstepSource == null) 
                footstepSource = GetComponent<AudioSource>();
        }


        private void PlayStepSound()
        {
            if (groundCheck == null)
                return;

            AudioClip clip = null;

            GroundSurface surface = groundCheck.GetGroundType();

            switch (surface)
            {
                case GroundSurface.Snow:
                    clip = snowFS[Random.Range(0, snowFS.Count)];
                    //Debug.Log("Play snow footsteps");
                    break;

                case GroundSurface.Dirt:
                    clip = dirtFS[Random.Range(0, dirtFS.Count)];
                    //Debug.Log("Play dirt footsteps");
                    break;

                case GroundSurface.Rock:
                    clip = rockFS[Random.Range(0, rockFS.Count)];
                    //Debug.Log("Play rock footsteps");
                    break;
                case GroundSurface.Ice:
                    clip = iceFS[Random.Range(0, iceFS.Count)];
                    //Debug.Log("Play ice footsteps");
                    break;

                default:
                    
                    break;
            }
            footstepSource.clip = clip;
            footstepSource.volume = Random.Range(0.02f, 0.04f);
            footstepSource.pitch = Random.Range(0.45f, 0.55f);
            footstepSource.Play();
        }
    }
}