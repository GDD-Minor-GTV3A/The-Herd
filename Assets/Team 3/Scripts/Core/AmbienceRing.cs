using System.Collections;
using UnityEngine;

public class AmbienceRing : MonoBehaviour
{
    [Tooltip("The library of sounds that get randomly played in the current surroundings.")]
    [SerializeField] private AudioClip[] ambientSounds;

    [Header("Sound Settings")]
    [Tooltip("How far away from the origin should the sounds be played.")]
    [SerializeField] private int distance = 5;

    [Tooltip("The lower bound of the random delay after each ambiance sound plays.")]
    [SerializeField] private int lowerBound = 5;

    [Tooltip("The upper bound of the random delay after each ambiance sound plays.")]
    [SerializeField] private int upperBound = 12;


    private bool firstTime = true;

    IEnumerator TimedPlay()
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 location = this.transform.position + (direction * distance);
        AudioClip ambiance = ambientSounds[Random.Range(0, ambientSounds.Length)];
        AudioSource.PlayClipAtPoint(ambiance, location);
        yield return new WaitForSeconds(ambiance.length + Random.Range(lowerBound,upperBound));
        StartCoroutine(TimedPlay());

    }

    public void LoadAudioList(AudioClip[] zoneAmbience)
    {
        ambientSounds = zoneAmbience;
        if (firstTime)
        {
            StartCoroutine(TimedPlay());
            firstTime = false;
        }
  
    }
}
