using UnityEngine;

namespace Core.Audio
{
    /// <summary>
    /// ScriptableObject that stores audio clip configuration data.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClipData", menuName = "Audio/Audio Clip Data")]
    public class AudioClipData : ScriptableObject
    {
        [SerializeField][Tooltip("The audio clip to play.")]
        private AudioClip _audioClip;

        [SerializeField][Tooltip("Volume level for this audio clip (0-1).")]
        [Range(0f, 1f)]
        private float _volume = 1f;

        [SerializeField][Tooltip("Minimum pitch variation.")]
        [Range(0.1f, 3f)]
        private float _minPitch = 1f;

        [SerializeField][Tooltip("Maximum pitch variation.")]
        [Range(0.1f, 3f)]
        private float _maxPitch = 1f;

        [SerializeField][Tooltip("Whether this audio clip should loop.")]
        private bool _loop = false;


        /// <summary>
        /// The audio clip to play.
        /// </summary>
        public AudioClip AudioClip => _audioClip;

        /// <summary>
        /// Volume level for this audio clip (0-1).
        /// </summary>
        public float Volume => _volume;

        /// <summary>
        /// Minimum pitch variation.
        /// </summary>
        public float MinPitch => _minPitch;

        /// <summary>
        /// Maximum pitch variation.
        /// </summary>
        public float MaxPitch => _maxPitch;

        /// <summary>
        /// Whether this audio clip should loop.
        /// </summary>
        public bool Loop => _loop;
    }
}
