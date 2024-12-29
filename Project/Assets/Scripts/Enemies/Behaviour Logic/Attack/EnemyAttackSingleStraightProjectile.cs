using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single Straight Projectile", menuName = "Enemy Logic/Attack Logic/Single Straight Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase
{
    [SerializeField] private Rigidbody2D projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float _timeBetweenShots = 2f;

    private float _timer;
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
        _timer = _timeBetweenShots;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (_isAttacking) return;

        if (_timer >= _timeBetweenShots)
        {
            _isAttacking = true;
            _timer = 0f;
            attack = ShootProjectile();
        }

        if (!enemy.IsWithinStrikingDistance)
        {
            _timer = 0f;
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        _timer += Time.deltaTime;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    private async Task ShootProjectile()
    {
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        await Task.Delay((int)(0.5f * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;
        Rigidbody2D bullet = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
        bullet.gameObject.GetComponent<Projectile>()?.Initialize(enemy.GetComponent<Collider2D>(), direction);
        bullet.velocity = direction * projectileSpeed;
        await Task.Delay((int)(0.2f * 1000));
        _isAttacking = false;
        enemy.GetComponentInChildren<SpriteRenderer>().color = _startingColor;
        //DestroyProjectile(bullet.gameObject);
    }

    private async void DestroyProjectile(GameObject projectile)
    {
        await Task.Delay((int)(3.0f * 1000));
        DestroyImmediate(projectile);
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
