using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Follow-Direct Follow", menuName = "Enemy Logic/Follow Logic/Direct Follow")]
public class EnemyFollowDirectToPlayer : EnemyFollowSOBase
{
    [SerializeField] private float _movementSpeed = 1.1f;
    [SerializeField] private float _outOfSightCooldown = 0.5f;
    private Transform PointA;
    private Transform PointB;
    private float _outOfSightTimer = 0f;

    public override void DoAnimationATriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationATriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        PointA = (enemy as Grub).PointA;
        PointB = (enemy as Grub).PointB;
        //enemy.audioManager.PlayAudioClip("Move", true, 0.08f);
    }

    public override void DoExitLogic()
    {
       //enemy.audioManager.StopAudioClip("Move");
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (OutOfLimitsCheck())
        {
            enemy.gameObject.GetComponent<IEnemyMoveable>()?.MoveEnemy(Vector2.zero);
        }
        else
        {
            Vector2 moveDirection = (playerTransform.position - enemy.transform.position).normalized;
            (enemy as Grub).ChangeDirection(moveDirection.x < 0);
            enemy.gameObject.GetComponent<IEnemyMoveable>()?.MoveEnemy(moveDirection * _movementSpeed);
        }
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

    private bool OutOfLimitsCheck()
    {
        if (enemy.transform.position.x < PointA.position.x)
        {
            return true;
        }
        if (enemy.transform.position.x > PointB.position.x)
        {
            return true;
        }
        return false;
    }
}
