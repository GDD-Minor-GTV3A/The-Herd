using UnityEngine;

public class DrekavacAudioController
{
    private readonly AudioSource _audioSource;
    
    private readonly AudioClip _screech;
    private readonly AudioClip _chomp;
    private readonly AudioClip _snarl;

    public DrekavacAudioController(AudioSource audioSource, AudioClip screech, AudioClip chomp, AudioClip snarl)
    {
        _audioSource = audioSource;
        _screech = screech;
        _chomp = chomp;
        _snarl = snarl;
    }

    public void PlayScreech()
    {
        PlayClip(_screech);
    }

    public void PlayChomp()
    {
        PlayClip(_chomp);
    }

    public void PlaySnarl()
    {
        PlayClip(_snarl);
    }

    private void PlayClip(AudioClip clip)
    {
        if (_audioSource is null || clip is null) return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }
}