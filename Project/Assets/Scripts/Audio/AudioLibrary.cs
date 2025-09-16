using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public abstract class AudioLibraryBase : ScriptableObject
{
    public abstract Type EnumType { get; }
    public abstract CustomAudioClip GetClip(Enum key);
}


public abstract class AudioLibrary<TEnum> : AudioLibraryBase where TEnum : Enum
{
    [Tooltip("Map each enum value to an AudioClip.")]
    [SerializedDictionary("Key", "Clip")]
    public SerializedDictionary<TEnum, CustomAudioClip> clips;

    public override Type EnumType => typeof(TEnum);

    public override CustomAudioClip GetClip(Enum key)
    {
        if (key is TEnum tkey && clips.TryGetValue(tkey, out var clip))
        {
            return clip;
        }

        return null;
    }

    public CustomAudioClip GetClip(TEnum key)
        => clips.TryGetValue(key, out var clip) ? clip : null;
}


[Serializable]
public class CustomAudioClip
{
    public AudioClip clip;

    [Range(0.1f, 1f)]
    public float volume = 1f;
}

