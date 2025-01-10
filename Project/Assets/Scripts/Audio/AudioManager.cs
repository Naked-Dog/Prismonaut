using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    public AudioList sounds;
    private GameObject audioContainer;
    private List<AudioSource> audioSources = new();

    public AudioManager(GameObject parent, AudioList audioList)
    {
        sounds = audioList;
        audioContainer = new GameObject();
        audioContainer.transform.parent = parent.transform;
        CreateNewAudioSource();
    }

    private void CreateNewAudioSource()
    {
        GameObject audioObject = new GameObject();
        audioObject.transform.parent = audioContainer.transform;
        AudioSource newAudioSource = audioObject.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        audioSources.Add(newAudioSource);        
    }

    private AudioSource GetFreeAudioSource()
    {
        foreach(AudioSource audioSource in audioSources)
        {
            if(!audioSource.isPlaying)
            {
                return audioSource;
            }
        }

        return null;
    }

    public void PlayAudioClip(AudioClip clip, bool isLoop = false)
    {
        AudioSource freeAudioSource = GetFreeAudioSource();

        if(freeAudioSource == null) 
        {
            CreateNewAudioSource();
            PlayAudioClip(clip);
        }

        freeAudioSource.clip = clip;
        freeAudioSource.loop = isLoop;
        freeAudioSource.Play();
    }

    public void StopAudioClip(AudioClip clip)
    {
        foreach(AudioSource audioSource in audioSources)
        {
            if(audioSource.clip == clip && audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.loop = false;
                return;
            }
        }
    }


}
