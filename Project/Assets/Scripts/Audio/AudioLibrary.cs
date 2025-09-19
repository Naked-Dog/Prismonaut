using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public abstract class AudioLibraryBase : ScriptableObject
{
    public abstract Type EnumType { get; }
    public abstract CustomAudioClip GetClip(Enum key);
    public abstract CustomAudioClip GetCustomClipByAudioClip(AudioClip clip);
}


public abstract class AudioLibrary<TEnum> : AudioLibraryBase where TEnum : Enum
{
    [Tooltip("Map each enum value to an AudioClip.")]
    [SerializedDictionary("Key", "Clip")]
    public SerializedDictionary<TEnum, CustomAudioClip> clips;

    //Rever dictionary for a instantanus an instantaneous search for a clip
    private Dictionary<AudioClip, CustomAudioClip> clipLookup;

    public override Type EnumType => typeof(TEnum);

    private void OnEnable()
    {
        BuildLookup();
    }

    private void OnValidate()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        clipLookup = new Dictionary<AudioClip, CustomAudioClip>();

        foreach (var kvp in clips)
        {
            if (kvp.Value?.clip != null && !clipLookup.ContainsKey(kvp.Value.clip))
            {
                clipLookup[kvp.Value.clip] = kvp.Value;
            }
        }
    }


    public override CustomAudioClip GetClip(Enum key)
    {
        if (key is TEnum tkey && clips.TryGetValue(tkey, out var clip))
        {
            return clip;
        }

        return null;
    }

    public CustomAudioClip GetClip(TEnum key) => clips.TryGetValue(key, out var clip) ? clip : null;

    public override CustomAudioClip GetCustomClipByAudioClip(AudioClip clip)
    {
        if (clip == null) return null;
        clipLookup.TryGetValue(clip, out var customClip);
        return customClip;
    }
}


[Serializable]
public class CustomAudioClip
{
    public AudioClip clip;

    [Range(0.1f, 1f)]
    public float volume = 1f;
}

