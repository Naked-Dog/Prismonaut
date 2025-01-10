
using System;
using PlayerSystem;
using UnityEngine;

public class PlayerAudioModule : AudioManager
{
    private EventBus eventBus;

    public PlayerAudioModule(EventBus eventBus, AudioList audioList, GameObject parent) : base(parent, audioList)
    {
        this.eventBus = eventBus;
        sounds = audioList;

        eventBus.Subscribe<PlayPlayerSounEffect>(ReproduceSoundEffect);
    }

    private void ReproduceSoundEffect(PlayPlayerSounEffect e)
    {
        PlayAudioClip(e.clip);
    }
}
