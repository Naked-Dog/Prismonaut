using UnityEngine;

public class ScarabullAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private ScarabullAnimationState currentState;

    public void SetState(ScarabullAnimationState newState)
    {
        if (newState == currentState) return;

        switch (newState)
        {
            case ScarabullAnimationState.Awake:
                Debug.Log("Awake animation triggered");
                animator.Play("Awake");
                break;
            case ScarabullAnimationState.Idle:
                animator.Play("Idle");
                break;
            case ScarabullAnimationState.Walk:
                animator.Play("Walk");
                break;
            case ScarabullAnimationState.Charging:
                animator.Play("Charging");
                break;
            case ScarabullAnimationState.Rush:
                animator.Play("Rush");
                break;
            case ScarabullAnimationState.Flinch:
                animator.Play("Flinch");
                break;
            case ScarabullAnimationState.Restore:
                animator.Play("Restore");
                break;
            case ScarabullAnimationState.Shockwave:
                animator.Play("Shockwave");
                break;
            case ScarabullAnimationState.RockSlide:
                animator.Play("Shockwave");
                break;
            case ScarabullAnimationState.Death:
                animator.Play("Death");
                break;
        }

        currentState = newState;
    }

    public void HurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }
}
