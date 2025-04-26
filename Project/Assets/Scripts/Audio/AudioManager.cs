using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager: MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    [Header("Pool settings")]
    [SerializeField] private int initialPoolSize = 10;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxMixer;   
    [SerializeField] private AudioMixerGroup musicMixer;   

    [Header("Audio Libraries")]
    [SerializeField] private AudioLibraryBase[] libraries;
    private Dictionary<Type, AudioLibraryBase> libraryMap = new();


    public Dictionary<string, AudioClip> clips;
    private List<AudioSource> allSources = new();
    private Stack<AudioSource> freeSources = new();

    protected void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for(int i = 0; i < initialPoolSize; i++)
        {
            freeSources.Push(CreateSource());
        }

        RegisterAudioLibraries();
    }

    private void RegisterAudioLibraries()
    {
        foreach (var lib in libraries)
        {
            libraryMap[lib.EnumType] = lib;
        }
    }

    private AudioSource CreateSource()
    {
        var go = new GameObject("PooledAudio");
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        allSources.Add(src);
        return src;
    }

    private AudioSource GetSource()
    {
        return freeSources.Count > 0 ? freeSources.Pop() : CreateSource();
    }

    private void ReleaseSource(AudioSource src)
    {
        src.clip = null;
        src.loop = false;
        src.outputAudioMixerGroup = null;
        src.transform.SetParent(transform);
        freeSources.Push(src);
    }

    private IEnumerator RecycleWhenDone(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        ReleaseSource(src);
    }

    public void RegisterClip(string name, AudioClip clip)
    {
        clips[name] = clip;
    }

    public AudioSource Play<TEnum>(TEnum key, float volume = 1f, bool loop = false) where TEnum : Enum
    {
        if (!libraryMap.TryGetValue(typeof(TEnum), out var baseLib))
        {
            return null;
        }

        var lib = baseLib as AudioLibrary<TEnum>;
        var clip = lib?.GetClip(key);

        if (clip == null){
            return null;
        } 

        var src = GetSource();
        src.clip = clip;
        src.volume = volume;
        src.loop = loop;
        src.outputAudioMixerGroup = sfxMixer;
        src.spatialBlend = 1f;
        src.Play();
        if (!loop) StartCoroutine(RecycleWhenDone(src));
        return src;
    }

    public AudioSource PlayAtPosition<TEnum>(TEnum key, Vector3 position, float volume = 1f, bool loop = false) where TEnum : Enum
    {
        var src = Play(key, volume, loop);
        if (src != null) src.transform.position = position;
        return src;
    }

    public AudioSource PlayAttached<TEnum>(TEnum key, Transform parent, float volume = 1f, bool loop = false) where TEnum : Enum
    {
        var src = Play(key, volume, loop);

        if (src != null)
        {
            src.transform.SetParent(parent);
            src.transform.localPosition = Vector3.zero;
        }
        
        return src;
    }

    public void Stop(string name)
    {
        foreach (var src in allSources)
        {
            if (src.isPlaying && src.clip != null && src.clip.name == name)
            {
                src.Stop();
            }
        }
    }

    public void StopAll()
    {
        foreach (var src in allSources)
        {
            if (src.isPlaying) src.Stop();
        }
    }
}
