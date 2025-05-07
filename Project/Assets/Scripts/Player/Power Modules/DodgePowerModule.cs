using System;
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
        private Vector2 inputDirection = Vector2.zero;
        private Vector2 appliedDirection = Vector2.zero;
        private bool isFacingRight => playerState.facingDirection == Direction.Right;
        private float cancelDotThreshold = 0.5f;

        public DodgePowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            Collider2D dodgeCollider,
            Collider2D playerCollider)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.dodgeCollider = dodgeCollider;
            this.playerCollider = playerCollider;

            movementValues = GlobalConstants.Get<PlayerPowersScriptable>();
            dodgeCollider.enabled = false;

            eventBus.Subscribe<OnCirclePowerInput>(OnCirclePowerInput);
            eventBus.Subscribe<OnHorizontalInput>(OnHorizontaInput);
            eventBus.Subscribe<OnVerticalInput>(OnVerticalInput);
        }

        private void OnHorizontaInput(OnHorizontalInput e)
        {
            inputDirection.x = e.amount;
        }

        private void OnVerticalInput(OnVerticalInput e)
        {
            inputDirection.y = e.amount;
        }

        private void OnCirclePowerInput(OnCirclePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            Activate();
        }

        private void Activate()
        {
            dodgeCollider.enabled = true;
            playerCollider.enabled = false;
            appliedDirection = inputDirection.sqrMagnitude > 0.1f
                ? inputDirection.normalized
                : (isFacingRight ? Vector2.right : Vector2.left);

            Vector2 dodgeImpulse = appliedDirection * movementValues.dodgePowerForce;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.AddForce(dodgeImpulse, ForceMode2D.Impulse);

            eventBus.Publish(new OnDodgeActivation());
            eventBus.Publish(new RequestMovementPause());
            playerState.activePower = Power.Dodge;

            playerState.powerTimeLeft = movementValues.dodgePowerDuration;
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Subscribe<OnFixedUpdate>(CheckCancelByShapeCast);
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

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (0 < playerState.powerTimeLeft) return;
            Deactivate(false);
        }

        private void Deactivate(bool force)
        {
            playerState.activePower = Power.None;
            playerCollider.enabled = true;
            dodgeCollider.enabled = false;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Unsubscribe<OnFixedUpdate>(CheckCancelByShapeCast);
            eventBus.Publish(new RequestMovementResume());
            if(force)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.AddForce(Vector2.up * movementValues.forceCancelImpulse, ForceMode2D.Impulse);
            }
        }
    }
}