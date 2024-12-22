using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ITriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public Rigidbody2D RigidBody { get; set; }
    public bool IsFacingRight { get; set; } = true;

    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }


    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyFollowState FollowState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyStunState StunState { get; set; }

    #endregion

    #region Scriptable Object Variables

    [SerializeField] private EnemyIdleSOBase EnemyIdleBase;

    [SerializeField] private EnemyFollowSOBase EnemyFollowBase;

    [SerializeField] private EnemyAttackSOBase EnemyAttackBase;
    [SerializeField] private EnemyStunSOBase EnemyStunBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
    public EnemyFollowSOBase EnemyFollowBaseInstance { get; set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
    public EnemyStunSOBase EnemyStunBaseInstance { get; set; }

    #endregion

    private void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
        if (EnemyFollowBase != null) EnemyFollowBaseInstance = Instantiate(EnemyFollowBase);
        EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);
        if (EnemyStunBase != null) EnemyStunBaseInstance = Instantiate(EnemyStunBase);

        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        FollowState = new EnemyFollowState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        StunState = new EnemyStunState(this, StateMachine);
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;

        RigidBody = GetComponent<Rigidbody2D>();

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        if (EnemyFollowBase != null) EnemyFollowBaseInstance.Initialize(gameObject, this);
        EnemyAttackBaseInstance.Initialize(gameObject, this);
        if (EnemyStunBase != null) EnemyStunBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Health / Die Functions
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Movement Functions
    public void MoveEnemy(Vector2 velocity)
    {
        RigidBody.velocity = velocity;
        CheckForLeftOrRightFacing(velocity);
    }

    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {
        if (IsFacingRight && velocity.x < 0f)
        {
            Vector3 rotator = new(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
        else if (!IsFacingRight && velocity.x > 0f)
        {
            Vector3 rotator = new(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
    }
    #endregion

    #region Distance Checks
    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }
    #endregion

    #region Animation Triggers
    private void AnimationTriggerEvent(AnimationTriggerType animationTriggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(animationTriggerType);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
    }
    #endregion
}