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


    public AudioDictionary clips;
    public AudioSource audioSourceTemplate;
    private GameObject audioContainer;
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
        //clips[name] = clip;
    }

    public AudioSource Play(string name, float volume = 1f, bool loop = false)
    {
        //if (!clips.TryGetValue(name, out var clip)) return null;
        var src = GetSource();
        //src.clip = clip;
        src.volume = volume;
        src.loop = loop;
        src.outputAudioMixerGroup = sfxMixer;
        src.spatialBlend = 1f;
        src.Play();
        if (!loop) StartCoroutine(RecycleWhenDone(src));
        return src;
    }

    public AudioSource PlayAtPosition(string name, Vector3 position, float volume = 1f, bool loop = false)
    {
        var src = Play(name, volume, loop);
        if (src != null) src.transform.position = position;
        return src;
    }

    public AudioSource PlayAttached(string name, Transform parent, float volume = 1f, bool loop = false)
    {
        var src = Play(name, volume, loop);
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

    // private AudioSource CreateNewAudioSource()
    // {
    //     GameObject audioObject = new GameObject();
    //     audioObject.transform.parent = audioContainer.transform;
    //     audioObject.transform.localPosition = Vector2.zero;
    //     AudioSource newAudioSource = audioObject.AddComponent<AudioSource>();
    //     newAudioSource.playOnAwake = false;

    //     if (audioSourceTemplate)
    //     {
    //         newAudioSource.spatialBlend = audioSourceTemplate.spatialBlend;
    //         newAudioSource.spread = audioSourceTemplate.spread;
    //         newAudioSource.rolloffMode = audioSourceTemplate.rolloffMode;
    //         newAudioSource.minDistance = audioSourceTemplate.minDistance;
    //         newAudioSource.maxDistance = audioSourceTemplate.maxDistance;
    //     }

    //     allSources.Add(newAudioSource);
    //     return newAudioSource;
    // }

    // private AudioSource GetFreeAudioSource()
    // {
    //     foreach (AudioSource audioSource in allSources)
    //     {
    //         if (!audioSource.isPlaying)
    //         {
    //             return audioSource;
    //         }
    //     }

    //     return CreateNewAudioSource();

    // }

    // public void PlayAudioClip(string clipName, bool isLoop = false, float volume = 1)
    // {
    //     AudioClip clip = clips.GetAudioClip(clipName);
    //     if (clip == null) return;

    //     AudioSource freeAudioSource = GetFreeAudioSource();

    //     freeAudioSource.clip = clip;
    //     freeAudioSource.loop = isLoop;
    //     freeAudioSource.volume = volume;
    //     freeAudioSource.Play();
    // }

    // public void StopAudioClip(string clipName)
    // {
    //     AudioClip clip = clips.GetAudioClip(clipName);

    //     foreach (AudioSource audioSource in allSources)
    //     {
    //         if (audioSource.clip == clip && audioSource.isPlaying)
    //         {
    //             audioSource.Stop();
    //             audioSource.clip = null;
    //             audioSource.loop = false;
    //             return;
    //         }
    //     }
    // }

    // public void StopAllAudioClips()
    // {
    //     foreach (AudioSource audioSource in allSources)
    //     {
    //         if (audioSource.isPlaying)
    //         {
    //             audioSource.Stop();
    //             audioSource.clip = null;
    //             audioSource.loop = false;
    //             return;
    //         }
    //     }
    // }
}
