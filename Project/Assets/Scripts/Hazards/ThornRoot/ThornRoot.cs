using DG.Tweening;
using UnityEngine;

public enum ThornState { Idle = 0, Charge, Attack, Broken }
public class ThornRoot : MonoBehaviour
{
    private ThornState thornState = ThornState.Idle;
    [SerializeField] private float chargeTime;
    [SerializeField] private float prevAttackTime;
    [SerializeField] private float attackDuration;
    [SerializeField] private float brokenDuration;

    [SerializeField] private GameObject hitBox;
    [SerializeField] private Animator animator;
    [SerializeField] private ThornHazard thornHazard;
    private Sequence currSequence;

    void Start()
    {
        thornHazard.onParry += () => ChangeState(ThornState.Broken);
        ChangeState(ThornState.Idle);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        if (thornState == ThornState.Idle)
        {
            ChangeState(ThornState.Charge);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        if (thornState == ThornState.Charge)
        {
            ChangeState(ThornState.Idle);
        }
    }
    private void ChangeState(ThornState newState)
    {
        thornState = newState;

        switch (thornState)
        {
            case ThornState.Idle:
                IdleSequence();
                break;

            case ThornState.Charge:
                ChargeSequence();
                break;

            case ThornState.Attack:
                AttackSequence();
                break;

            case ThornState.Broken:
                OnBreak();
                break;
        }
    }
    private void IdleSequence()
    {
        currSequence?.Kill();
        animator.Play("Idle");
    }
    private void ChargeSequence()
    {
        animator.Play("Charge");
        currSequence = DOTween.Sequence();
        currSequence.Append(DOVirtual.DelayedCall(chargeTime, () => ChangeState(ThornState.Attack), false));
    }
    private void AttackSequence()
    {
        currSequence?.Kill();
        currSequence = DOTween.Sequence();

        animator.Play("PrevAttackGlow");
        currSequence.AppendInterval(prevAttackTime);

        currSequence.AppendCallback(() =>
        {
            animator.Play("StartAttack");
        });

        currSequence.AppendInterval(attackDuration);

        currSequence.AppendCallback(() =>
        {
            animator.Play("EndAttack");
        });
    }

    private void OnBreak()
    {
        currSequence.Kill();

        animator.Play("Break");

        DOVirtual.DelayedCall(brokenDuration, () =>
        {
            animator.Play("Recover");
        }, false);
    }
    public void ResetThorn() => ChangeState(ThornState.Idle);

    public void SetHitBox(bool active)
    {
        hitBox.SetActive(active);
        float transformX = active ? 3f : 0.5f;
        if(hitBox.transform.localScale.x != transformX)
            hitBox.transform.DOScaleX(transformX, 0.1f);
    }
}
