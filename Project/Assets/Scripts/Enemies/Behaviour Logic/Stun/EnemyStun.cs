using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun-Stun", menuName = "Enemy Logic/Stun Logic/Stun")]

public class EnemyStun : EnemyStunSOBase
{
    [SerializeField] private float _stunDuration = 3.0f;
    private Color _startingColor;
    private float _damageAmount;
    private Task stun;


    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        enemy.RigidBody.bodyType = RigidbodyType2D.Kinematic;
        stun = Stun();
        enemy.audioManager.StopAudioClip("Move");
    }

    public override void DoExitLogic()
    {
        enemy.RigidBody.bodyType = RigidbodyType2D.Dynamic;
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    private async Task Stun()
    {
        enemy.SetAggroStatus(false);
        enemy.SetStrikingDistanceBool(false);
        enemy.MoveEnemy(Vector2.zero);
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        enemy.audioManager.PlayAudioClip("Stun");
        await Task.Delay((int)(_stunDuration * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = _startingColor;
        enemy.StateMachine.ChangeState(enemy.IdleState);
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
        _startingColor = enemy.GetComponentInChildren<SpriteRenderer>().color;
    }
    public override void ResetValues()
    {
        base.ResetValues();
    }
}
