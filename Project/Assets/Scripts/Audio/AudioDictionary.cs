using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class AudioDictionary : MonoBehaviour
{   
    private Dictionary<string, AudioClip> sounds;

    public void AttachSounds(SerializedDictionary<string,AudioClip> soundList)
    {
        sounds = soundList;
    }

    public AudioClip GetAudioClip(string clipName)
    {
        if(sounds.TryGetValue(clipName, out var clip))
        {
            return clip;
        }

        return null;
    }
}
