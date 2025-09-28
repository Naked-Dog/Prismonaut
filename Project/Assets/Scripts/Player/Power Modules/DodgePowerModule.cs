using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace PlayerSystem
{
    public class DodgePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PlayerPowersScriptable movementValues;
        private Collider2D dodgeCollider;
        private PhysicsEventsRelay dodgePhysicsRelay;
        private Collider2D playerCollider;
        private PlayerBaseModule baseModule;
        private Vector2 inputDirection = Vector2.zero;
        private Vector2 appliedDirection = Vector2.zero;
        private bool isFacingRight => playerState.facingDirection == Direction.Right;
        private float cancelDotThreshold = 0.5f;
        private float dashTimer = 0f;
        private bool insideEnemy = false;

        public DodgePowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            Collider2D dodgeCollider,
            Collider2D playerCollider,
            PlayerBaseModule baseModule)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.dodgeCollider = dodgeCollider;
            this.playerCollider = playerCollider;
            this.baseModule = baseModule;

            dodgePhysicsRelay = dodgeCollider.GetComponent<PhysicsEventsRelay>();
            movementValues = GlobalConstants.Get<PlayerPowersScriptable>();
            dodgeCollider.enabled = false;

            eventBus.Subscribe<OnCirclePowerInput>(OnCirclePowerInput);
            eventBus.Subscribe<OnHorizontalInput>(OnHorizontaInput);
            eventBus.Subscribe<OnVerticalInput>(OnVerticalInput);
        }

        private void OnHorizontaInput(OnHorizontalInput e)
        {
            if (Mathf.Abs(e.amount) < 0.25f)
                inputDirection.x = 0;
            else
                inputDirection.x = Mathf.Sign(e.amount);
        }

        private void OnVerticalInput(OnVerticalInput e)
        {
            if (Mathf.Abs(e.amount) < 0.25f)
                inputDirection.y = 0;
            else
                inputDirection.y = e.amount;
        }

        private void OnCirclePowerInput(OnCirclePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            Activate();
        }

        private void Activate()
        {
            if (playerState.currentCharges < 1f) return;
            baseModule.StartChargeRegeneration();
            dodgeCollider.enabled = true;
            playerCollider.enabled = false;
            appliedDirection = inputDirection.sqrMagnitude > 0.1f
                ? inputDirection.normalized
                : (isFacingRight ? Vector2.right : Vector2.left);


            eventBus.Publish(new OnDodgeActivation());
            eventBus.Publish(new RequestMovementPause());
            eventBus.Publish(new RequestGravityOff());
            playerState.activePower = Power.Dodge;

            playerState.powerTimeLeft = movementValues.dodgePowerDuration;
            eventBus.Subscribe<OnFixedUpdate>(FixedImpulse);
            eventBus.Subscribe<OnFixedUpdate>(CheckCancelByShapeCast);
            eventBus.Subscribe<OnFixedUpdate>(CheckEnemyCollision);
            dodgePhysicsRelay.OnCollisionEnter2DAction.AddListener(CheckCollsion);
        }

        private void CheckCollsion(Collision2D col)
        {
            if(col.gameObject.CompareTag("Slime"))
            {
                Deactivate();
            }
        }

        private void FixedImpulse(OnFixedUpdate e)
        {
            dashTimer += Time.fixedDeltaTime;
            float tNorm = dashTimer / movementValues.dodgePowerDuration;

            if (tNorm <= 1f)
            {
                float speedNow = movementValues.dodgePowerForce * (1f - tNorm);
                rb2d.linearVelocity = appliedDirection * speedNow;
            }
            else
            {
                Deactivate();
            }
        }

        private void CheckCancelByShapeCast(OnFixedUpdate e)
        {
            CircleCollider2D circle = dodgeCollider as CircleCollider2D;
            float radius = circle.radius * dodgeCollider.transform.lossyScale.x;
            Vector2 origin = dodgeCollider.transform.TransformPoint(dodgeCollider.offset);
            Vector2 dir = appliedDirection.normalized;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, radius, dir, 0.1f, movementValues.circleCastLayerMask);
            foreach (var hit in hits)
            {
                float dot = Vector2.Dot(hit.normal, -dir);
                if (dot > cancelDotThreshold)
                {
                    Deactivate(true);
                    return;
                }
            }
        }

        private void CheckEnemyCollision(OnFixedUpdate e)
        {
            var boxColl = playerCollider as BoxCollider2D;
            Vector2 origin = (Vector2)boxColl.transform.position + boxColl.offset;
            Vector2 size = boxColl.size;
            float angle = boxColl.transform.eulerAngles.z;

            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, size, angle, movementValues.enemyLayerMask);
            insideEnemy = hits.Any(c => c.CompareTag("Enemy"));
        }

        private void Deactivate(bool force = false)
        {
            if (insideEnemy)
            {
                Vector2 repel = Vector2.up * 10f;
                rb2d.AddForce(repel, ForceMode2D.Impulse);
                insideEnemy = false;
            }


            dashTimer = 0f;
            playerCollider.enabled = true;
            dodgeCollider.enabled = false;

            eventBus.Unsubscribe<OnFixedUpdate>(FixedImpulse);
            eventBus.Unsubscribe<OnFixedUpdate>(CheckCancelByShapeCast);
            eventBus.Unsubscribe<OnFixedUpdate>(CheckEnemyCollision);
            eventBus.Publish(new RequestMovementResume());
            eventBus.Publish(new RequestGravityOn());

            if (force)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.AddForce(Vector2.up * movementValues.forceCancelImpulse, ForceMode2D.Impulse);
                eventBus.Publish(new OnCancelPower());
                AudioManager.Instance?.Stop(PlayerSoundsEnum.DodgeTrans);
                return;
            }

            playerState.activePower = Power.None;
        }
    }
}