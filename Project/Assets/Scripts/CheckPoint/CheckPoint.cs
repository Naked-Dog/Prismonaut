using PlayerSystem;
using UnityEngine;

public class CheckPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationEventEmitter animationEvent;
    [SerializeField] private bool interactOnEnter;
    [SerializeField] private bool destroyOnInteract;
    private bool isOpen;

    public bool IsInteractable => !isOpen;
    public bool InteractOnEnter => interactOnEnter;
    public bool DestroyOnInteract => destroyOnInteract;

    private void Start()
    {
        animationEvent.OnAnimationEventTriggered += HandleAnimationEvent;
    }

    private void HandleAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "Idle":
                AudioManager.Instance.Play("Idle", 0.35f, true);
                break;
            case "Deploy":
                AudioManager.Instance.Play("Deploy");
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
