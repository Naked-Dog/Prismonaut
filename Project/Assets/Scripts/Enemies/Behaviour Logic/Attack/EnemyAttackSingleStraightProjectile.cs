using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single Straight Projectile", menuName = "Enemy Logic/Attack Logic/Single Straight Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase
{
    [SerializeField] private Rigidbody2D projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;

    private float _timer;
    private float _timeBetweenShots = 2f;

    /*     private float _exitTimer;
        private float _timeTillExit = 3f;
        private float _distanceToCountExit = 3f; */

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

        if (_timer > _timeBetweenShots)
        {
            _timer = 0f;

            Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;

            Rigidbody2D bullet = GameObject.Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
            bullet.velocity = direction * projectileSpeed;
            DestroyProjectileAfterDelay(bullet.gameObject);
        }

        if (!enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        /* if (Vector2.Distance(playerTransform.position, enemy.transform.position) > _distanceToCountExit)
        {
            _exitTimer += Time.deltaTime;

            if (_exitTimer > _timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.FollowState);
            }
        }
        else
        {
            _exitTimer = 0f;
        } */
        _timer += Time.deltaTime;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    private async void DestroyProjectileAfterDelay(GameObject projectile)
    {
        await Task.Delay((int)(3.0f * 1000));
        Destroy(projectile);
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }
}
