using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single Straight Projectile", menuName = "Enemy Logic/Attack Logic/Single Straight Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase
{
    [SerializeField] private Rigidbody2D projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float _timeBetweenShots = 1f;

    /* private float minAngle = -30f;
    private float maxAngle = 30f; */

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
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _timer = 0f;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (_isAttacking) return;
        Vector2 direction = playerTransform.position - enemy.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        (enemy as Spitter).mouthTransform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        if (_timer >= _timeBetweenShots)
        {
            _isAttacking = true;
            _timer = 0f;
            attack = ShootProjectile(direction, angle);
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

    private async Task ShootProjectile(Vector2 direction, float angle)
    {
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        await Task.Delay((int)(0.3f * 1000));
        enemy.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        Rigidbody2D bullet = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
        bullet.gameObject.GetComponent<Projectile>()?.Initialize(enemy.GetComponent<Collider2D>(), direction.normalized);
        bullet.transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
        bullet.velocity = direction.normalized * projectileSpeed;
        enemy.audioManager.PlayAudioClip("Shoot");
        await Task.Delay((int)(0.2f * 1000));
        _isAttacking = false;
        enemy.GetComponentInChildren<SpriteRenderer>().color = _startingColor;
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
        attack?.Dispose();
    }
}
