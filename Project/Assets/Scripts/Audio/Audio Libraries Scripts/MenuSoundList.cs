using AYellowpaper.SerializedCollections;
using UnityEngine;

public class MenuSoundList : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> menuSoundList;

    protected void Awake()
    {
        //AttachSounds(menuSoundList);
    }
}
