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

    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject hurtBox;
    [SerializeField] private SpriteRenderer spriteRenderer;
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
                SetToIdle();
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
    private void SetToIdle()
    {
        currSequence?.Kill();
        spriteRenderer.color = Color.green;
        //returning to idle animation
        //idle animation loop
    }
    private void ChargeSequence()
    {
        spriteRenderer.color = Color.yellow;
        currSequence = DOTween.Sequence();
        currSequence.Append(DOVirtual.DelayedCall(chargeTime, () => ChangeState(ThornState.Attack), false));
    }
    private void AttackSequence()
    {
        currSequence?.Kill();
        currSequence = DOTween.Sequence();

        currSequence.Append(spriteRenderer.DOColor(Color.white, prevAttackTime));

        currSequence.AppendCallback(() =>
        {
            spriteRenderer.color = Color.red;
            Transform hurtBoxT = hurtBox.transform;

            hurtBoxT.localScale = new Vector2(0, hurtBoxT.localScale.y);
            hurtBoxT.DOScaleX(3.5f, 0.15f);

            hurtBox.SetActive(true);
        });

        currSequence.AppendInterval(attackDuration);

        currSequence.AppendCallback(() =>
        {
            hurtBox.SetActive(false);
            ChangeState(ThornState.Idle);
        });

    }
    private void OnBreak()
    {
        currSequence.Kill();
        hurtBox.SetActive(false);

        Color transparentColor = Color.white;
        transparentColor.a = 0.5f;
        spriteRenderer.color = transparentColor;

        boxCollider.enabled = false;

        DOVirtual.DelayedCall(brokenDuration, () =>
        {
            boxCollider.enabled = true;
            ChangeState(ThornState.Idle);
        }, false);
    }
}
