using UnityEngine;
using System.Collections.Generic;

namespace PlayerSystem
{
    public class Physics2DMovement : PlayerMovement
    {
        private Rigidbody2D rb2d;
        private PlayerState playerState;
        private List<Collision2D> collisions = new List<Collision2D>();
        private bool jumpRequested = false;
        private float requestedMovement = 0f;
        private bool landingRequested = false;
        private float jumpCooldown = 0f;
        private float landingMoveCooldown = 0f;

        readonly float maxHorizonalVelocity = 8f;
        readonly float maxJumpCooldown = 0.2f;
        readonly float maxLandingBreakCooldown = 0.1f;
        private bool pauseMovement = false;

        public Physics2DMovement(
            EventBus eventBus,
            PlayerState playerState,
            PlayerMovementScriptable movementValues,
            Rigidbody2D rb2d)
            : base(eventBus, movementValues)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;

            eventBus.Subscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Subscribe<OnJumpInput>(OnJumpInput);
            eventBus.Subscribe<OnCollisionEnter2D>(OnCollisionEnter);
            eventBus.Subscribe<OnCollisionStay2D>(OnCollisionStay);
            eventBus.Subscribe<CollisionExit2D>(OnCollisionExit);
            eventBus.Subscribe<OnUpdate>(OnUpdate);
            eventBus.Subscribe<OnFixedUpdate>(OnFixedUpdate);
            eventBus.Subscribe<RequestMovementPause>(RequestMovementPause);
            eventBus.Subscribe<RequestMovementResume>(RequestMovementResume);
            eventBus.Subscribe<RequestGravityOff>(RequestGravityOff);
            eventBus.Subscribe<RequestGravityOn>(RequestGravityOn);
        }

        private void RequestMovementPause(RequestMovementPause e)
        {
            eventBus.Unsubscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Unsubscribe<OnJumpInput>(OnJumpInput);
            pauseMovement = true;
        }

        private void RequestMovementResume(RequestMovementResume e)
        {
            eventBus.Subscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Subscribe<OnJumpInput>(OnJumpInput);
            pauseMovement = false;
        }

        private void OnCollisionEnter(OnCollisionEnter2D e)
        {
            collisions.Add(e.collision);
        }

        private void OnCollisionStay(OnCollisionStay2D e)
        {
            DoGroundCheck();
        }

        private void OnCollisionExit(CollisionExit2D e)
        {
            collisions.Remove(e.collision);
            DoGroundCheck();
        }

        private void DoGroundCheck()
        {
            if (landingRequested) return;
            if (0f < jumpCooldown) return;
            bool hasGroundedContact = false;
            foreach (Collision2D collision in collisions)
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (0.9f < contact.normal.y && 0.01f < contact.normalImpulse)
                    {
                        hasGroundedContact = true;
                        break;
                    }
                }
                if (hasGroundedContact) break;
            }
            if (!hasGroundedContact)
            {
                playerState.groundState = GroundState.Airborne;
                Debug.Log(collisions);
            }
            if (!playerState.groundState.Equals(GroundState.Grounded) && hasGroundedContact) landingRequested = true;
        }

        protected override void OnHorizontalInput(OnHorizontalInput e)
        {
            if (e.amount == 0f) return;
            if (isMovementDisabled) return;
            if (requestedMovement != 0f) return;
            if (0 < landingMoveCooldown) return;
            requestedMovement = e.amount * 5f;
        }

        protected override void OnJumpInput(OnJumpInput e)
        {
            if (isJumpingDisabled) return;
            if (jumpRequested) return;
            if (!playerState.groundState.Equals(GroundState.Grounded)) return;
            if (0f < jumpCooldown) return;
            jumpRequested = true;

        }

        private void OnUpdate(OnUpdate e)
        {
            playerState.velocity = rb2d.linearVelocity;
        }

        private void OnFixedUpdate(OnFixedUpdate e)
        {
            if (jumpRequested) PerformJump();
            if (landingRequested) PerformLanding();
            if (pauseMovement) return;
            if (requestedMovement != 0f) PerformMovement();
            else PerformBreak();
        }

        private void PerformJump()
        {
            Vector2 impulseVector = new Vector2(0, 12f - rb2d.linearVelocity.y);
            rb2d.AddForce(impulseVector, ForceMode2D.Impulse);
            playerState.groundState = GroundState.Airborne;
            jumpCooldown = maxJumpCooldown;
            eventBus.Subscribe<OnUpdate>(ReduceJumpCooldown);
            jumpRequested = false;
        }

        private void PerformMovement()
        {
            requestedMovement *= movementValues.horizontalVelocity;
            float forceToApply;
            float velocityDirection = Mathf.Sign(rb2d.linearVelocity.x * requestedMovement);
            if (0f < velocityDirection)
            {
                float lerpAmount = Mathf.Abs(rb2d.linearVelocity.x) / maxHorizonalVelocity;
                forceToApply = velocityDirection < 0f ? requestedMovement : Mathf.Lerp(requestedMovement, 0f, lerpAmount);
            }
            else
            {
                forceToApply = requestedMovement;
            }
            rb2d.AddForce(forceToApply * Vector2.right);
            if (Mathf.Sign(requestedMovement * rb2d.linearVelocity.x) < 0) PerformBreak();
            eventBus.Publish(new OnHorizontalMovement(requestedMovement));
            requestedMovement = 0f;
        }

        private void PerformBreak()
        {
            float breakModifier = playerState.groundState == GroundState.Airborne ? movementValues.airBreak : movementValues.groundBreak;
            float breakForce = -rb2d.linearVelocity.x * breakModifier;
            rb2d.AddForce(Vector2.right * breakForce, ForceMode2D.Force);
        }

        private void PerformLanding()
        {
            playerState.groundState = GroundState.Grounded;
            landingRequested = false;
            if (0 < requestedMovement * rb2d.linearVelocity.x) return;
            rb2d.AddForce(Vector2.right * -rb2d.linearVelocity.x * 0.75f, ForceMode2D.Impulse);
            landingMoveCooldown = maxLandingBreakCooldown;
            eventBus.Subscribe<OnUpdate>(ReduceLandingMoveCooldown);
        }

        private void ReduceJumpCooldown(OnUpdate e)
        {
            jumpCooldown -= Time.deltaTime;
            if (jumpCooldown <= 0f) eventBus.Unsubscribe<OnUpdate>(ReduceJumpCooldown);
        }

        private void ReduceLandingMoveCooldown(OnUpdate e)
        {
            landingMoveCooldown -= Time.deltaTime;
            if (landingMoveCooldown <= 0f) eventBus.Unsubscribe<OnUpdate>(ReduceLandingMoveCooldown);
        }

        private void RequestGravityOff(RequestGravityOff e)
        {
            rb2d.gravityScale = 0f;
        }
        private void RequestGravityOn(RequestGravityOn e)
        {
            rb2d.gravityScale = movementValues.gravityScale;
        }
    }
}