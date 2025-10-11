using System.Collections.Generic;
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
        [Tooltip("Sound clips for steps sounds.")]
        [SerializeField] private List<AudioClip> _walkingSounds; /// for private use _camelCase naming, for public PascalCase
        [Tooltip("Sound source for steps.")]
        [SerializeField] private AudioSource _footstepSource;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize() /// we dont use Start() and Awake(), instead use Initialize()
        {
            if (_footstepSource == null)
                _footstepSource = GetComponent<AudioSource>();
        }


        private void PlayStepSound()
        {
            AudioClip clip;
            clip = _walkingSounds[Random.Range(0, _walkingSounds.Count)];
            _footstepSource.clip = clip;
            _footstepSource.volume = Random.Range(0.02f, 0.05f);
            _footstepSource.pitch = Random.Range(0.8f, 1.2f);
            _footstepSource.Play();
        }
    }
}