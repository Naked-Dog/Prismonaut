using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class EnemyGrubSoundList : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> enemyGrubSounds;

    protected void Awake()
    {
        //AttachSounds(enemyGrubSounds);
    }
}
