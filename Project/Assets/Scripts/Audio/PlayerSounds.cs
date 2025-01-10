using AYellowpaper.SerializedCollections;
using UnityEngine;

public class PlayerSounds : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> playerSounds;

    protected void Awake()
    {
        AttachSounds(playerSounds);
    }
}


   
