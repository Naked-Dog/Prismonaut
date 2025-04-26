using AYellowpaper.SerializedCollections;
using UnityEngine;

public class PortalSoundList : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> portalSoundList;

    protected void Awake()
    {
        //AttachSounds(portalSoundList);
    }
}
