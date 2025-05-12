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


    public Dictionary<string, AudioClip> clips = new();
    private List<AudioSource> allSources = new();
    private Stack<AudioSource> freeSources = new();
    private AudioSource musicSource;

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

        musicSource = CreateMusicSource();
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

    private AudioSource CreateMusicSource()
    {
        var go = new GameObject("Music Source");
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = true;
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

    public AudioSource PlaySound<TEnum>(TEnum key, float volume = 1f, bool loop = false, float spatialBlend = 1) where TEnum : Enum
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
        src.spatialBlend = spatialBlend;
        src.spread = 180;
        src.minDistance = 0.07f;
        src.maxDistance = 0.15f;
        src.Play();
        if (!loop) StartCoroutine(RecycleWhenDone(src));
        return src;
    }



    public AudioSource Play3DSountAtPosition<TEnum>(TEnum key, Vector3 position, float volume = 1f, bool loop = false) where TEnum : Enum
    {
        var src = PlaySound(key, volume, loop);
        if (src != null) src.transform.position = position;
        return src;
    }

    public AudioSource Play3DSoundAttached<TEnum>(TEnum key, Transform parent, float volume = 1f, bool loop = false) where TEnum : Enum
    {
        var src = PlaySound(key, volume, loop);

        if (src != null)
        {
            src.transform.SetParent(parent);
            src.transform.localPosition = Vector3.zero;
        }
        
        return src;
    }

    public AudioSource Play2DSound<TEnum>(TEnum key, float volume = 1, bool loop = false) where TEnum : Enum
    {
        var src = PlaySound(key, volume, loop, 0);
        return src;
    }

    public AudioSource Play2DSoundByLibrary<TEnum>(
        TEnum key,
        AudioLibrary<TEnum> library,
        float volume = 1f,
        bool loop = false
    ) where TEnum : Enum
    {
        if (library == null) return null;
        var clip = library.GetClip(key);
        if (clip == null) return null;

        var src = GetSource();
        src.clip = clip;
        src.volume = volume;
        src.loop = loop;
        src.outputAudioMixerGroup = sfxMixer;
        src.spatialBlend = 0f;
        src.Play();
        if (!loop) StartCoroutine(RecycleWhenDone(src));
        return src;
    }

    public void Stop<TEnum>(TEnum key) where TEnum : Enum
    {
        if (!libraryMap.TryGetValue(typeof(TEnum), out var baseLib))
        {
            Debug.LogWarning($"No se encontró la librería de audio para {typeof(TEnum).Name}");
            return;
        }

        var lib = baseLib as AudioLibrary<TEnum>;
        var clip = lib?.GetClip(key);
        if (clip == null)
        {
            Debug.LogWarning($"No se encontró el clip para el enum {key}");
            return;
        }

        foreach (var src in allSources)
        {
            if (src.isPlaying && src.clip == clip)
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

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public AudioSource PlayMusic<TEnum>(TEnum key, float volume = 1) where TEnum : Enum
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

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.Play();
        return musicSource;
    }
}
