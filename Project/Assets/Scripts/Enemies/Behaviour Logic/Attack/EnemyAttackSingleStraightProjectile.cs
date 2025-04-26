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
        /* float maxValue;
        float minValue;
        if ((enemy as Spitter).weakness == PlayerSystem.Power.Triangle)
        {
            maxValue = 10f;
            minValue = 320f;
        }
        else
        {
            if (direction.x > 0f)
            {
                maxValue = 10f;
                minValue = 320f;
            }
            else
            {
                maxValue = 80f;
                minValue = 30f;
            }
        }
        Quaternion localRotation = LookAt(playerTransform, minValue, maxValue); */
        /* Vector2 direction = playerTransform.position - enemy.transform.position;
        float angleModifier;
        Vector3 rotationDirection;
        float maxValue;
        float minValue;
        if ((enemy as Spitter).weakness == PlayerSystem.Power.Triangle)
        {
            angleModifier = -180f;
            rotationDirection = Vector3.back;
            maxValue = 25f;
            minValue = -25f;
        }
        else
        {
            angleModifier = direction.x > 0f ? -90f : 90f;
            rotationDirection = direction.x > 0f ? Vector3.forward : Vector3.back;
            if (direction.x > 0f)
            {
                maxValue = 25f;
                minValue = -25f;
            }
            else
            {
                maxValue = 205f;
                minValue = 155f;
            }
        }
        //Debug.Log("angle" + (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        float angle = Mathf.Clamp(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, minValue, maxValue);
        //Debug.Log("angle after clamp: " + angle);
        //Debug.Log("angle - modifier: " + (angle - angleModifier));
        (enemy as Spitter).mouthTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // Debug.Log("mouth rotation: " + (enemy as Spitter).mouthTransform.rotation); */
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
    private Quaternion LookAt(Transform target, float minAngle, float maxAngle)
    {
        if ((enemy as Spitter).mouthTransform.rotation.eulerAngles.z < 20f || (enemy as Spitter).mouthTransform.rotation.eulerAngles.z > 70f)
            return (enemy as Spitter).mouthTransform.localRotation;
        Debug.Log("euler rotation: " + (enemy as Spitter).mouthTransform.rotation.eulerAngles.z);
        // Calculate direction to the target
        //Vector3 direction = target.position - (enemy as Spitter).mouthTransform.position;
        Vector2 direction = target.position - enemy.transform.position;
        // Calculate the desired rotation (without clamping or parent compensation)
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        // Compensate for parent rotation
        Quaternion parentRotationInverse = Quaternion.Inverse(enemy.transform.rotation);
        Quaternion localTargetRotation = parentRotationInverse * targetRotation;

        // Get the desired local rotation as Euler angles
        Vector3 localEulerAngles = localTargetRotation.eulerAngles;

        // Clamp the local rotation around the Y-axis (for 2D)
        /* localEulerAngles.z = Mathf.Clamp(localEulerAngles.z, minAngle, maxAngle);
        localEulerAngles.x = 0;
        localEulerAngles.y = 0; */
        //localEulerAngles.z = ClampAngle(localEulerAngles.z, minAngle, maxAngle);
        // Create the clamped local rotation as a Quaternion
        localTargetRotation = Quaternion.Euler(0, 0, localEulerAngles.z);

        // Apply the clamped local rotation
        (enemy as Spitter).mouthTransform.localRotation = localTargetRotation;
        Debug.Log("final z: " + (enemy as Spitter).mouthTransform.localRotation.eulerAngles.z);
        return localTargetRotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (min > max)
        {
            // Handle the case where min is greater than max (e.g., 355, 5)
            if (angle < min && angle > max)
            {
                return angle; // Angle is within the valid range
            }
            else if (angle <= max)
            {
                return max;
            }
            else
            {
                return min;
            }
        }
        else
        {
            // Standard clamping for min <= max
            return Mathf.Clamp(angle, min, max);
        }
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
        Rigidbody2D bullet = Instantiate(projectilePrefab, (enemy as Spitter).projectileOrigin.position, Quaternion.identity);
        bullet.gameObject.GetComponent<Projectile>()?.Initialize(enemy.GetComponent<Collider2D>(), direction.normalized);
        bullet.transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
        bullet.linearVelocity = direction.normalized * projectileSpeed;
        //enemy.audioManager.PlayAudioClip("Shoot");
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
