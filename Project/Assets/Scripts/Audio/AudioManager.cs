using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    public AudioDictionary sounds;
    private GameObject audioContainer;
    private List<AudioSource> audioSources = new();

    public AudioManager(GameObject parent, AudioDictionary audioList)
    {
        sounds = audioList;
        audioContainer = new GameObject();
        audioContainer.transform.parent = parent.transform;
        CreateNewAudioSource();
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject audioObject = new GameObject();
        audioObject.transform.parent = audioContainer.transform;
        AudioSource newAudioSource = audioObject.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        audioSources.Add(newAudioSource);
        return newAudioSource;        
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

        return CreateNewAudioSource();
        
    }

    public void PlayAudioClip(string clipName, bool isLoop = false)
    {
        AudioClip clip = sounds.GetAudioClip(clipName);
        if(clip == null) return;

        AudioSource freeAudioSource = GetFreeAudioSource();

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
