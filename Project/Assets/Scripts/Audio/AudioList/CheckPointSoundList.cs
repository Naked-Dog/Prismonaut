using AYellowpaper.SerializedCollections;
using UnityEngine;

public class CheckPointSoundList : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> checkPointSoundList;

    protected void Awake()
    {
        AttachSounds(checkPointSoundList);
    }
}
