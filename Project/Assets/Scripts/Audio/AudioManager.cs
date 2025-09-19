using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public enum SoundsType
{
    Music,
    Sfxs,
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Pool settings")]
    [SerializeField] private int initialPoolSize = 10;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup musicMixer;

    [Header("Audio Libraries")]
    [SerializeField] private AudioLibraryBase[] libraries;
    private Dictionary<Type, AudioLibraryBase> libraryMap = new();

    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float SoundEffectsVolume = 0.8f;

    public Dictionary<string, AudioClip> clips = new();
    private List<AudioSource> allSources = new();
    private Stack<AudioSource> freeSources = new();
    private AudioSource musicSource;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (!(Instance == this)) Destroy(gameObject);
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
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

    public AudioSource PlaySound<TEnum>(TEnum key, bool loop = false, float spatialBlend = 1) where TEnum : Enum
    {
        if (!libraryMap.TryGetValue(typeof(TEnum), out var baseLib))
        {
            return null;
        }

        var lib = baseLib as AudioLibrary<TEnum>;
        var customClip = lib?.GetClip(key);

        if (customClip == null)
        {
            return null;
        }

        var src = GetSource();
        src.clip = customClip.clip;
        src.volume = SoundEffectsVolume * customClip.volume;
        src.loop = loop;
        src.outputAudioMixerGroup = sfxMixer;
        src.spatialBlend = spatialBlend;
        src.spread = 180;
        src.minDistance = 10f;
        src.maxDistance = 0.15f;
        src.Play();
        if (!loop) StartCoroutine(RecycleWhenDone(src));
        return src;
    }



    public AudioSource Play3DSountAtPosition<TEnum>(TEnum key, Vector3 position, bool loop = false) where TEnum : Enum
    {
        var src = PlaySound(key, loop);
        if (src != null) src.transform.position = position;
        return src;
    }

    public AudioSource Play3DSoundAttached<TEnum>(TEnum key, Transform parent, bool loop = false) where TEnum : Enum
    {
        var src = PlaySound(key, loop);
        if (src != null)
        {
            src.transform.SetParent(parent);
            src.transform.localPosition = Vector3.zero;
        }

        return src;
    }

    public AudioSource Play2DSound<TEnum>(TEnum key, bool loop = false) where TEnum : Enum
    {
        var src = PlaySound(key, loop, 0);
        return src;
    }

    public AudioSource Play2DSoundByLibrary<TEnum>(
        TEnum key,
        AudioLibrary<TEnum> library,
        bool loop = false
    ) where TEnum : Enum
    {
        if (library == null) return null;
        var customClip = library.GetClip(key);
        if (customClip == null) return null;

        var src = GetSource();
        src.clip = customClip.clip;
        src.volume = SoundEffectsVolume * customClip.volume;
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
        var customClip = lib?.GetClip(key);
        if (customClip == null)
        {
            Debug.LogWarning($"No se encontró el clip para el enum {key}");
            return;
        }

        foreach (var src in allSources)
        {
            if (src.isPlaying && src.clip == customClip.clip)
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

    public AudioSource PlayMusic<TEnum>(TEnum key) where TEnum : Enum
    {
        if (!libraryMap.TryGetValue(typeof(TEnum), out var baseLib))
        {
            return null;
        }

        var lib = baseLib as AudioLibrary<TEnum>;
        var customClip = lib?.GetClip(key);

        if (customClip == null)
        {
            return null;
        }

        musicSource.clip = customClip.clip;
        musicSource.volume = musicVolume * customClip.volume;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.Play();
        return musicSource;
    }

    public void ChangeVolume(SoundsType type, float value)
    {
        switch (type)
        {
            case SoundsType.Music:
                musicVolume = value;

                if (musicSource != null && musicSource.isPlaying)
                {
                    var clipVol = 1f;
                    if (musicSource.clip != null)
                    {
                        foreach (var lib in libraryMap.Values)
                        {
                            var customClip = lib.GetCustomClipByAudioClip(musicSource.clip);
                            if (customClip != null)
                            {
                                clipVol = customClip.volume;
                                break;
                            }
                        }
                    }
                    musicSource.volume = musicVolume * clipVol;
                }

                break;
            case SoundsType.Sfxs:
                SoundEffectsVolume = value;

                foreach (var src in allSources)
                {
                    if (src.isPlaying && src.outputAudioMixerGroup == sfxMixer)
                    {
                        var clipVol = 1f;
                        foreach (var lib in libraryMap.Values)
                        {
                            var customClip = lib.GetCustomClipByAudioClip(src.clip);
                            if (customClip != null)
                            {
                                src.volume = SoundEffectsVolume * customClip.volume;
                                break;
                            }
                        }
                        src.volume = SoundEffectsVolume * clipVol;
                    }
                }
                break;
            default:
                break;
        }
    }

    public float GetMusicVolume() { return musicVolume; }
    public float GetSFXsVolume() { return SoundEffectsVolume; }
}
