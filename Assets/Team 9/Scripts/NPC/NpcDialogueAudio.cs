using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class NPCDialogueAudio : MonoBehaviour
{
    [Header("Audio Configuration")]
    [Tooltip("Drag the Player's AudioSource (or any '2D' source) here. If left empty, it tries to use an AudioSource on this NPC.")]
    [SerializeField] private AudioSource _targetAudioSource;

    [Header("Talking Sound Settings")]
    [SerializeField] private AudioClip _talkingSoundClip;
    [SerializeField] private AudioMixerGroup _talkingSoundMixerGroup;
    [SerializeField] private int _soundPlayEveryNCharacters = 2; 

    private void Awake()
    {
        // FALLBACK: If you forgot to drag the Player's source in, 
        // it defaults to looking for one on the NPC itself so errors don't happen.
        if (_targetAudioSource == null)
        {
            _targetAudioSource = GetComponent<AudioSource>();
            if (_targetAudioSource == null)
            {
                _targetAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void PlayTypingSound(int characterIndex, char currentChar)
    {
        if (_talkingSoundClip == null || _targetAudioSource == null)
        {
            return;
        }
        
        // Play sound only every N characters and skip silent ones
        if (characterIndex % _soundPlayEveryNCharacters == 0 && 
            currentChar != '.' && currentChar != ' ' && currentChar != ',' && 
            currentChar != '!' && currentChar != '?')
        {
            // 1. Force the Mixer Group (in case the Player source is set to SFX or something else)
            if (_talkingSoundMixerGroup != null)
            {
                _targetAudioSource.outputAudioMixerGroup = _talkingSoundMixerGroup;
            }

            // 2. Randomize pitch slightly
            _targetAudioSource.pitch = UnityEngine.Random.Range(0.99f, 1.01f);
            
            // 3. Play the clip defined ON THIS NPC, but through the TARGET source
            _targetAudioSource.PlayOneShot(_talkingSoundClip);
        }
    }
}