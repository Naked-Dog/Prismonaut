using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class EnemySpitterSoundList : AudioDictionary
{
    [SerializedDictionary("AudioName", "AudiClip")]
    public SerializedDictionary<string, AudioClip> enemySpitterSounds;

    protected void Awake()
    {
        AttachSounds(enemySpitterSounds);
    }
}
