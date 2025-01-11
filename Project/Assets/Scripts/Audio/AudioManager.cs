using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    public AudioDictionary sounds;
    public AudioSource  audioSourceTemplate;
    private GameObject audioContainer;
    private List<AudioSource> audioSources = new();

    public AudioManager(GameObject parent, AudioDictionary audioList, AudioSource audioSourceTemplate)
    {
        sounds = audioList;
        this.audioSourceTemplate = audioSourceTemplate;
        audioContainer = new GameObject("AudiosContainer");
        audioContainer.transform.parent = parent.transform;
        audioContainer.transform.localPosition = Vector2.zero;
        CreateNewAudioSource();
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject audioObject = new GameObject();
        audioObject.transform.parent = audioContainer.transform;
        audioObject.transform.localPosition = Vector2.zero;
        AudioSource newAudioSource = audioObject.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;

        if(audioSourceTemplate)
        {
            newAudioSource.spatialBlend = audioSourceTemplate.spatialBlend;
            newAudioSource.spread = audioSourceTemplate.spread;
            newAudioSource.rolloffMode = audioSourceTemplate.rolloffMode;
            newAudioSource.minDistance = audioSourceTemplate.minDistance;
            newAudioSource.maxDistance = audioSourceTemplate.maxDistance;
        }

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

    public void PlayAudioClip(string clipName, bool isLoop = false, float volume = 1)
    {
        AudioClip clip = sounds.GetAudioClip(clipName);
        if(clip == null) return;

        AudioSource freeAudioSource = GetFreeAudioSource();

        freeAudioSource.clip = clip;
        freeAudioSource.loop = isLoop;
        freeAudioSource.volume = volume;
        freeAudioSource.Play();
    }

    public void StopAudioClip(string clipName)
    {
        AudioClip clip = sounds.GetAudioClip(clipName);

        foreach(AudioSource audioSource in audioSources)
        {
            if(audioSource.clip == clip && audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
                return;
            }
        }
    }

}
