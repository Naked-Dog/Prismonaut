using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class MusicList : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> musicList;

    protected void Awake()
    {
        //AttachSounds(musicList);
    }
}
