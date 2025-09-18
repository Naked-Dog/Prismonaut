using UnityEngine;
using System.Collections.Generic;
using CameraSystem;
using Cinemachine;
using System;

namespace PlayerSystem
{
    public class CollisionSnapshot
    {
        public Collider2D otherCollider;
        public List<ContactPoint2D> contacts = new();
        public Vector2 relativeVelocity;
        public float timestamp;

        public CollisionSnapshot(Collision2D collision)
        {
            otherCollider = collision.collider;
            contacts.AddRange(collision.contacts); // store all contact points
            relativeVelocity = collision.relativeVelocity;
            timestamp = Time.time;
        }
    }

    public class Physics2DMovement : PlayerMovement
    {
        private Rigidbody2D rb2d;
        private PlayerState playerState;
        private Dictionary<Collider2D, CollisionSnapshot> collisions = new();
        private bool jumpRequested = false;
        private bool jumpReleased = false;
        private float jumpForce;
        private float jumpGravity;
        private float jumpTimer;
        private float requestedMovement = 0f;
        private float landingMoveCooldown = 0f;
        private float groundedGraceTimer = 0f;
        private PlayerBaseModule baseModule;
        readonly float maxLandingBreakCooldown = 0.1f;
        private bool pauseMovement = false;

        public Physics2DMovement(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PlayerBaseModule baseModule)
            : base(eventBus)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.baseModule = baseModule;

            eventBus.Subscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Subscribe<OnJumpInput>(OnJumpInput);
            eventBus.Subscribe<OnCollisionEnter2D>(AddCollisionSnapshot);
            eventBus.Subscribe<OnCollisionStay2D>(UpdateCollisionSnapshot);
            eventBus.Subscribe<OnCollisionExit2D>(RemoveCollisionSnapshot);
            eventBus.Subscribe<OnFixedUpdate>(OnFixedUpdate);
            eventBus.Subscribe<RequestMovementPause>(RequestMovementPause);
            eventBus.Subscribe<RequestMovementResume>(RequestMovementResume);
            eventBus.Subscribe<RequestGravityOff>(RequestGravityOff);
            eventBus.Subscribe<RequestGravityOn>(RequestGravityOn);
            eventBus.Subscribe<RequestOppositeReaction>(RequestOppositeReaction);

            SetJumpValues();
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

        private void AddCollisionSnapshot(OnCollisionEnter2D e)
        {
            var snapshot = new CollisionSnapshot(e.collision);
            collisions[e.collision.collider] = snapshot;
        }

        void UpdateCollisionSnapshot(OnCollisionStay2D e)
        {
            collisions[e.collision.collider] = new CollisionSnapshot(e.collision);
        }

        private void RemoveCollisionSnapshot(OnCollisionExit2D e)
        {
            collisions.Remove(e.collision.collider);
        }

        private void DoGroundCheck()
        {
            bool hasGroundedContact = false;
            foreach (CollisionSnapshot snapshot in collisions.Values)
            {
                foreach (ContactPoint2D contact in snapshot.contacts)
                {
                    if (0.9f < contact.normal.y && 0.01f < contact.normalImpulse)
                    {
                        hasGroundedContact = true;
                        break;
                    }
                }
                if (hasGroundedContact) break;
            }

            if (hasGroundedContact)
            {
                groundedGraceTimer = movementConstants.groundedGracePeriod;
            }
            else
            {
                groundedGraceTimer -= Time.deltaTime;
                if (groundedGraceTimer < 0f) playerState.groundState = GroundState.Airborne;
            }

            if (!playerState.groundState.Equals(GroundState.Grounded) && hasGroundedContact) PerformLanding();
        }

        protected override void OnHorizontalInput(OnHorizontalInput e)
        {
            if (e.amount == 0f) return;
            if (isMovementDisabled) return;
            if (requestedMovement != 0f) return;
            if (0 < landingMoveCooldown) return;
            requestedMovement = e.amount;
        }

        protected override void OnJumpInput(OnJumpInput e)
        {
            if (e.context.canceled)
            {
                jumpReleased = true;
            }

            if (!CanJump()) return;

            if (e.context.performed)
            {
                jumpRequested = true;
            }

        }

        private void OnFixedUpdate(OnFixedUpdate e)
        {
            playerState.velocity = rb2d.linearVelocity;
            playerState.rotation = rb2d.rotation;
            DoGroundCheck();
            if (jumpRequested) PerformJump();
            if (pauseMovement) return;
            if (requestedMovement != 0f) PerformMovement();
            else PerformHorizontalBreak();
            PerformVerticalBreak();
        }

        private void SetJumpValues()
        {
            jumpForce = 2f * movementConstants.JumpHeight / movementConstants.JumpTimeToPeak;
            jumpGravity = -2f * movementConstants.JumpHeight / (movementConstants.JumpTimeToPeak * movementConstants.JumpTimeToPeak);
        }

        private void PerformJump()
        {
            eventBus.Publish(new RequestGravityOff());
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, jumpForce);
            eventBus.Subscribe<OnFixedUpdate>(PerformJumpGravity);

            jumpTimer = 0;
            eventBus.Subscribe<OnUpdate>(RunJumpTimer);
            eventBus.Subscribe<OnFixedUpdate>(ReleaseJump);

            playerState.groundState = GroundState.Airborne;
            jumpReleased = false;
            jumpRequested = false;

            AudioManager.Instance?.Play2DSound(PlayerSoundsEnum.Jump);
        }

        private void PerformJumpGravity(OnFixedUpdate e)
        {
            if (rb2d.linearVelocityY <= 0)
            { 
                eventBus.Publish(new RequestGravityOn());
                eventBus.Unsubscribe<OnFixedUpdate>(PerformJumpGravity);
            }
            
            float finalVerticalVelocity = rb2d.linearVelocityY + (jumpGravity * Time.fixedDeltaTime);
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, finalVerticalVelocity);
        }

        private void RunJumpTimer(OnUpdate e)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer > movementConstants.minJumpTime)
            {
                eventBus.Unsubscribe<OnUpdate>(RunJumpTimer);
            }
        }

        private void ReleaseJump(OnFixedUpdate e)
        {
            if (!jumpReleased) return;
            if (jumpTimer < movementConstants.minJumpTime) return;

            if (!playerState.groundState.Equals(GroundState.Airborne))
            {
                eventBus.Unsubscribe<OnFixedUpdate>(ReleaseJump);
                return;
            }

            CutJump();
        }

        private void CutJump()
        { 
            if (rb2d.linearVelocityY <= 0f) return;
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, 0);
            eventBus.Unsubscribe<OnFixedUpdate>(ReleaseJump);
        }

        private void PerformMovement()
        {
            requestedMovement *= playerState.groundState == GroundState.Grounded ?
                movementConstants.horizontalGroundedForce :
                movementConstants.horizontalAirborneForce;
            float forceToApply;
            float velocityDirection = Mathf.Sign(rb2d.linearVelocity.x * requestedMovement);
            if (0f < velocityDirection)
            {
                float lerpAmount = Mathf.Abs(rb2d.linearVelocity.x) / movementConstants.maxHorizontalVelocity;
                forceToApply = velocityDirection < 0f ? requestedMovement : Mathf.Lerp(requestedMovement, 0f, lerpAmount);
            }
            else
            {
                forceToApply = requestedMovement;
            }
            rb2d.AddForce(forceToApply * Vector2.right);
            if (Mathf.Sign(requestedMovement * rb2d.linearVelocity.x) < 0) PerformHorizontalBreak();
            eventBus.Publish(new OnHorizontalMovement(requestedMovement));
            requestedMovement = 0f;
        }

        private void PerformHorizontalBreak()
        {
            float breakModifier = playerState.groundState == GroundState.Airborne ? movementConstants.airBreak : movementConstants.groundBreak;
            float breakForce = -rb2d.linearVelocity.x * breakModifier;
            rb2d.AddForce(Vector2.right * breakForce, ForceMode2D.Force);
        }

        private void PerformVerticalBreak()
        {
            float threshold = Mathf.Abs(movementConstants.maxFallingVelocity);
            float excessVelocity = -rb2d.linearVelocity.y - threshold;
            if (excessVelocity <= 0) return;
            rb2d.AddForce(Vector2.up * excessVelocity, ForceMode2D.Impulse);
            baseModule.StartFallingCameraTimer();
        }

        private void PerformLanding()
        {
            playerState.groundState = GroundState.Grounded;
            baseModule.StopFallingCameraTimer();
            AudioManager.Instance?.Stop(PlayerSoundsEnum.LoopWindFall);
            AudioManager.Instance?.Play2DSound(PlayerSoundsEnum.Land);
            if (0 < requestedMovement * rb2d.linearVelocity.x) return;
            rb2d.AddForce(Vector2.right * -rb2d.linearVelocity.x * 0.75f, ForceMode2D.Impulse);
            landingMoveCooldown = maxLandingBreakCooldown;
            eventBus.Subscribe<OnUpdate>(ReduceLandingMoveCooldown);
            var standingCamera = CameraManager.Instance.SearchCamera(CineCameraType.Regular);
            CameraManager.Instance.ChangeCamera(standingCamera);
        }

        private void ReduceLandingMoveCooldown(OnUpdate e)
        {
            landingMoveCooldown -= Time.deltaTime;
            if (landingMoveCooldown <= 0f) eventBus.Unsubscribe<OnUpdate>(ReduceLandingMoveCooldown);
        }

        private void RequestGravityOff(RequestGravityOff e)
        {
            eventBus.Unsubscribe<OnFixedUpdate>(PerformJumpGravity);
            CutJump();
            rb2d.gravityScale = 0f;
        }
        private void RequestGravityOn(RequestGravityOn e)
        {
            rb2d.gravityScale = movementConstants.gravityScale;
        }

        private void RequestOppositeReaction(RequestOppositeReaction e)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.AddForce(e.direction * e.forceAmount, ForceMode2D.Impulse);
        }

        private bool CanJump()
        {
            if (!playerState.groundState.Equals(GroundState.Grounded)) return false;
            if (isJumpingDisabled) return false;
            if (jumpRequested) return false;

            return true;
        }
    }
}