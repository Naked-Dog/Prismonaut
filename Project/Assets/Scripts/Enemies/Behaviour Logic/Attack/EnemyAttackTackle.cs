using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Tackle", menuName = "Enemy Logic/Attack Logic/Tackle")]

public class EnemyAttackTackle : EnemyAttackSOBase
{
    [SerializeField] private float _tackleForce = 2f;
    [SerializeField] private float _attackCooldown = 2f;
    private float _timer = 0f;
    private float _timeTillExit = 2f;
    private bool _isAttacking = false;
    private Color _startingColor;
    private Task attack;

    public override void DoAnimationATriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationATriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (_isAttacking) return;
        if (enemy.isAttackInCooldown)
        {
            enemy.StateMachine.ChangeState(enemy.FollowState);
        }

        if (enemy.IsWithinStrikingDistance && enemy.IsAggroed)
        {
            _timer = 0f;
            _isAttacking = true;
            attack = Tackle();
        }

        if (!enemy.IsWithinStrikingDistance && enemy.IsAggroed)
        {
            if (_timer >= _timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.FollowState);
            }
            _timer += Time.deltaTime;
        }

        if (!enemy.IsWithinStrikingDistance && !enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }

    private async Task Tackle()
    {
        enemy.MoveEnemy(Vector2.zero);
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        await Task.Delay((int)(0.5f * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        float direction = enemy.IsFacingRight ? 1.0f : -1.0f;
        enemy.MoveEnemy(new Vector2(_tackleForce * direction, enemy.RigidBody.velocity.y));
        await Task.Delay((int)(0.35f * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = _startingColor;
        enemy.StartCoroutine(enemy.StartAttackCooldown());
        _isAttacking = false;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
        _startingColor = enemy.GetComponentInChildren<SpriteRenderer>().color;
    }

    private void OnDestroy()
    {
        if (!attack.IsCompleted)
        {
            attack.Dispose();
        }
    }
}
