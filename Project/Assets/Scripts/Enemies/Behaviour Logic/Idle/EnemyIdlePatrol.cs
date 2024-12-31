using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Patrol", menuName = "Enemy Logic/Idle Logic/Patrol")]
public class EnemyIdlePatrol : EnemyIdleSOBase
{
    [SerializeField] private float PatrolMovementSpeed = 1f;
    private Vector2 moveDirection;
    private Transform PointA;
    private Transform PointB;

    private Transform _currentTargetPoint;


    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        PointA = (enemy as Grub).PointA;
        PointB = (enemy as Grub).PointB;
        _currentTargetPoint = GetNextPosition();
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.FollowState);
        }
        moveDirection = (_currentTargetPoint.position - enemy.transform.position).normalized;
        enemy.GetComponentInChildren<SpriteRenderer>().flipX = moveDirection.x < 0;
        enemy.gameObject.GetComponent<IEnemyMoveable>()?.MoveEnemy(moveDirection * PatrolMovementSpeed);
        if ((enemy.transform.position - _currentTargetPoint.position).sqrMagnitude < 0.05f)
        {
            _currentTargetPoint = GetNextPosition();
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

    private Transform GetNextPosition()
    {
        return _currentTargetPoint == null || _currentTargetPoint == PointB ? PointA : PointB;
    }
}
