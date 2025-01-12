using PlayerSystem;
using UnityEngine;

public class CheckPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationEventEmitter animationEvent;
    private bool isOpen;

    private AudioManager audioManager;

    public bool IsInteractable => !isOpen;

    private void Start()
    {
        audioManager = new AudioManager(gameObject, GetComponent<CheckPointSoundList>(), GetComponent<AudioSource>());
        animationEvent.OnAnimationEventTriggered += HandleAnimationEvent;
    }

    private void HandleAnimationEvent(string eventName)
    {
        switch(eventName)
        {
            case "Idle":
                audioManager.PlayAudioClip("Idle", true, 0.35f);
                break;
            case "Deploy":
                audioManager.PlayAudioClip("Deploy");
                break;
        }
    }

    public void Interact()
    {
        GameDataManager.Instance?.SavePlayerPosition(transform.position);
        isOpen = true;
        animator.SetBool("isOpen", isOpen);
    }
}
