using UnityEngine;

public class ShamanSpawner : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcShaman;   
    
    [Header("Audio Settings")]
    public AudioSource shamanSound; 
    
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            SpawnShaman();
            ControlAudio();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (triggered && other.CompareTag("Player"))
        {
            triggered = false;
            DespawnShaman();
            RestoreAudio();
        }
    }

    private void SpawnShaman()
    {
        if (npcShaman != null)
        {
            npcShaman.SetActive(true);
        }
    }

    private void ControlAudio()
    {
        AudioManager audio = AudioManager.Instance;
        
        if (audio.musicSource != null)
            audio.musicSource.Stop();
        if (audio.sfxSource != null)
            audio.sfxSource.Stop();

        // Also stop all 3D sound boxes
        foreach (var box in audio.soundBoxes)
        {
            if (box != null)
                box.Stop();
        }

        // 🎵 Play ONLY the Shaman's sound
        if (shamanSound != null)
        {
            shamanSound.Play();
        }
    }
    
    private void RestoreAudio()
    {
        AudioManager audio = AudioManager.Instance;

        // Stop shaman audio
        if (shamanSound != null)
            shamanSound.Stop();

        // Resume background music
        if (audio.musicSource != null)
            audio.musicSource.Play();

        // Resume SFX source (optional)
        if (audio.sfxSource != null)
            audio.sfxSource.Play();

        // Restart 3D sound boxes
        foreach (var box in audio.soundBoxes)
        {
            if (box != null)
                box.Play();
        }
    }
    
    private void DespawnShaman()
    {
        if (npcShaman != null)
            npcShaman.SetActive(false);
    }
}
