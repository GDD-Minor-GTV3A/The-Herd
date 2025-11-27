using System.Collections.Generic;
using Core.Shared.Utilities;
using UnityEngine;

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
        private List<AudioClip> walkingSounds; /// for private use camelCase naming, for public PascalCase
        [SerializeField, Tooltip("Sound source for steps."), Required]
        private AudioSource footstepSource;


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
            AudioClip clip;
            clip = walkingSounds[Random.Range(0, walkingSounds.Count)];
            footstepSource.clip = clip;
            footstepSource.volume = Random.Range(0.02f, 0.05f);
            footstepSource.pitch = Random.Range(0.8f, 1.2f);
            footstepSource.Play();
        }
    }
}