using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogSound : MonoBehaviour
{
    public List<AudioClip> WalkingSounds;
    public AudioSource footstepSource;

    void Start()
    {
        footstepSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void DogWalking()
    {
        AudioClip clip;
        clip = WalkingSounds[Random.Range(0, WalkingSounds.Count)];
        footstepSource.clip = clip;
        footstepSource.volume = Random.Range(0.02f, 0.05f);
        footstepSource.pitch = Random.Range(0.8f, 1.2f);
        footstepSource.Play();
    }
}
