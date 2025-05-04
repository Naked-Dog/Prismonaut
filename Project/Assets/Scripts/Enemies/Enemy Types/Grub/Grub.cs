using System.Collections;
using PlayerSystem;
using UnityEngine;

public class Grub : Enemy, IEnemyMoveable, IPlayerPowerInteractable
{
    public Transform PointA;
    public Transform PointB;
    public Transform TriggersTransform;

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == PlayerSystem.Power.Square)
        {
            Damage(1);
        }
        if (playerState.activePower == PlayerSystem.Power.Circle)
        {
            StateMachine.ChangeState(StunState);
        }
    }
    #region Player Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            if (collision.gameObject.GetComponent<PlayerBaseModule>().state.activePower != PlayerSystem.Power.Square)
            {
                bool isStun = StateMachine.CurrentEnemyState == StunState;
                if (!isStun)
                {
                    bool playerDied = collision.gameObject.GetComponent<PlayerBaseModule>().healthModule.Damage(DamageAmount);
                    if (playerDied) return;
                }
                //collision.gameObject.GetComponent<PlayerBaseModule>().knockback.CallKnockback(direction, Vector2.up, Input.GetAxisRaw("Horizontal"), !isStun);
            }
            else
            {
                RigidBody.AddForce(new Vector2(-direction.x * 7f, 0), ForceMode2D.Impulse);
                if (isAttacking) StartCoroutine(TackleStun());
            }
        }
    }
    #endregion

    public void ChangeDirection(bool isChangeDirection)
    {
        GetComponentInChildren<SpriteRenderer>().flipX = isChangeDirection;
        TriggersTransform.localScale = new Vector2(isChangeDirection ? -1 : 1, 1);
    }

    private IEnumerator TackleStun()
    {
        yield return new WaitForSeconds(0.15f);
        StateMachine.ChangeState(StunState);
    }
}
