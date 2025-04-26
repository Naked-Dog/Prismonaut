
using System;
using PlayerSystem;
using UnityEngine;

public class PlayerAudioModule : AudioManager
{
    private EventBus eventBus;

    // public PlayerAudioModule(EventBus eventBus, AudioDictionary audioList, GameObject parent, AudioSource audioSourceTemplate) : base(parent, audioList, audioSourceTemplate)
    // {
    //     this.eventBus = eventBus;
    //     clips = audioList;
    //     this.audioSourceTemplate = audioSourceTemplate;

    //     eventBus.Subscribe<PlayPlayerSounEffect>(ReproduceSoundEffect);
    // }

    private void ReproduceSoundEffect(PlayPlayerSounEffect e)
    {
        AudioManager.Instance.Play(e.clipName, e.volume, false);
    }
}
