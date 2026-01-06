using UnityEngine;
using TMPro;

public class ShamanSpawner : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcShaman;
    public Animator shamanAnimator;
    public ShamanDialogueManager shamanDialogueManager;
    public ShamanAnimationHandler shamanAnimationHandler;
    public TextMeshProUGUI InterText;

    [Header("VFX")]
    public ParticleSystem spawnVFX;

    private ParticleSystem activeVFX;
    private bool triggered = false;
    public bool Triggered => triggered;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            InterText.enabled = true;
            SpawnShaman();
            StopGlobalAudio();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (triggered && other.CompareTag("Player"))
        {
            triggered = false;
            InterText.enabled = false;
            DespawnShaman();
            ResumeGlobalAudio();
            shamanDialogueManager.EndDialogue();

            if (shamanAnimationHandler != null && shamanAnimationHandler.shamanAudioSource != null)
            {
                shamanAnimationHandler.shamanAudioSource.Stop();
            }
        }
    }

    void Update()
    {
        if (triggered && Input.GetKeyDown(KeyCode.E))
        {
            shamanDialogueManager.StartDialogue();
            InterText.enabled = false;
            shamanAnimator.SetBool("isTalking", true);
        }
    }

    private void SpawnShaman()
    {
        if (npcShaman != null)
        {
            npcShaman.SetActive(true);

            if (spawnVFX != null)
            {
                activeVFX = Instantiate(
                    spawnVFX,
                    npcShaman.transform.position,
                    spawnVFX.transform.rotation
                );
                activeVFX.Play();
            }
        }
    }

    private void DespawnShaman()
    {
        if (npcShaman != null)
        {
            if (spawnVFX != null)
            {
                ParticleSystem vfx = Instantiate(
                    spawnVFX,
                    npcShaman.transform.position,
                    spawnVFX.transform.rotation
                );
                vfx.Play();
            }

            npcShaman.SetActive(false);
        }
    }

    // ---------------- GLOBAL AUDIO ----------------

    private void StopGlobalAudio()
    {
        AudioManager audio = AudioManager.Instance;
        if (audio == null) return;

        if (audio.musicSource != null)
            audio.musicSource.Stop();

        if (audio.sfxSource != null)
            audio.sfxSource.Stop();

        foreach (var box in audio.soundBoxes)
            box?.Stop();
    }

    private void ResumeGlobalAudio()
    {
        AudioManager audio = AudioManager.Instance;
        if (audio == null) return;

        if (audio.musicSource != null)
            audio.musicSource.Play();

        if (audio.sfxSource != null)
            audio.sfxSource.Play();

        foreach (var box in audio.soundBoxes)
            box?.Play();
    }
}
