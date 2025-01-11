using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationEventEmitter animationEvent;
    private bool isOpen;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = new AudioManager(gameObject, GetComponent<CheckPointSoundList>(), GetComponent<AudioSource>());
        animationEvent.OnAnimationEventTriggered += HandleAnimationEvent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerBaseModule>())
        {
            GameDataManager.Instance?.SavePlayerPosition(transform.position);
            isOpen = true;
            animator.SetBool("isOpen", isOpen);
        }
    }

    private void HandleAnimationEvent(string eventName)
    {
        switch(eventName)
        {
            case "Idle":
                audioManager.PlayAudioClip("Idle", true, 0.5f);
                break;
            case "Deploy":
                audioManager.PlayAudioClip("Deploy");
                break;
        }
    }
}
