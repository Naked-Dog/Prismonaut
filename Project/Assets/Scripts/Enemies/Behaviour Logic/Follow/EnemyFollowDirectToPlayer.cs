using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Follow-Direct Follow", menuName = "Enemy Logic/Follow Logic/Direct Follow")]
public class EnemyFollowDirectToPlayer : EnemyFollowSOBase
{
    [SerializeField] private float _movementSpeed = 1.1f;
    [SerializeField] private float _outOfSightCooldown = 0.5f;
    private float _outOfSightTimer = 0f;

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
        Vector2 moveDirection = (playerTransform.position - enemy.transform.position).normalized;
        enemy.gameObject.GetComponent<IEnemyMoveable>()?.MoveEnemy(moveDirection * _movementSpeed);
        if (enemy.IsWithinStrikingDistance && !enemy.isAttackInCooldown)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
        if (!enemy.IsAggroed)
        {
            _outOfSightTimer += Time.deltaTime;
            if (_outOfSightTimer >= _outOfSightCooldown) enemy.StateMachine.ChangeState(enemy.IdleState);
        }
        else
        {
            _outOfSightTimer = 0f;
        }
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }
}
