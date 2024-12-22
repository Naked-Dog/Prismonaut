using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Tackle", menuName = "Enemy Logic/Attack Logic/Tackle")]

public class EnemyAttackTackle : EnemyAttackSOBase
{
    [SerializeField] private float _tackleForce = 2f;
    private float _timer = 0f;
    private float _timeTillExit = 2f;
    private bool _isAttacking = false;
    private Color _startingColor;

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

        if (enemy.IsWithinStrikingDistance && enemy.IsAggroed)
        {
            _timer = 0f;
            _isAttacking = true;
            Tackle();
        }
        else if (!enemy.IsWithinStrikingDistance && enemy.IsAggroed)
        {
            if (_timer >= _timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.FollowState);
            }
            _timer += Time.deltaTime;
        }
        else if (!enemy.IsWithinStrikingDistance && !enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }

    private async void Tackle()
    {
        Debug.Log("Attack windup");
        enemy.MoveEnemy(Vector2.zero);
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        await Task.Delay((int)(0.5f * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        Debug.Log("Attacking");
        float direction = enemy.IsFacingRight ? 1.0f : -1.0f;
        enemy.MoveEnemy(new Vector2(_tackleForce * direction, enemy.RigidBody.velocity.y));
        await Task.Delay((int)(0.35f * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = _startingColor;
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
}
