using UnityEngine;
using CameraSystem;
using System.Collections.Generic;
using System;

namespace PlayerSystem
{
    public class Physics2DMovement : PlayerMovement
    {
        MonoBehaviour mb;
        public Rigidbody2D rb2d;

        private PlayerState playerState;
        private Collider2D coll;

        private bool _isFalling = false;
        public bool IsFalling
        {
            get => _isFalling;
            set
            {
                _isFalling = value;
                cameraState.CameraPosState = _isFalling ? CameraPositionState.Falling : CameraPositionState.Regular;
            }
        }
        private List<Collision2D> collisions = new List<Collision2D>();
        private CameraState cameraState;
        private PlayerAudioModule playerAudio;
        private bool jumpRequested = false;
        private float requestedMovement = 0f;
        private bool landingRequested = false;
        private float jumpCooldown = 0f;

        readonly float maxHorizonalVelocity = 8f;
        readonly float maxJumpCooldown = 0.2f;

        public Physics2DMovement(
            EventBus eventBus,
            PlayerState playerState,
            PlayerMovementScriptable movementValues,
            Rigidbody2D rb2d,
            TriggerEventHandler groundTrigger,
            CameraState cameraState,
            PlayerAudioModule playerAudio,
            MonoBehaviour mb) : base(eventBus, movementValues)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.playerAudio = playerAudio;
            this.mb = mb;
            this.cameraState = cameraState;
            coll = rb2d.GetComponent<Collider2D>();

            eventBus.Subscribe<HorizontalInputEvent>(OnMovementInput);
            eventBus.Subscribe<JumpInputEvent>(OnJumpInput);
            eventBus.Subscribe<CollisionEnterEvent>(OnCollisionEnter);
            eventBus.Subscribe<CollisionStayEvent>(OnCollisionStay);
            eventBus.Subscribe<CollisionExitEvent>(OnCollisionExit);
            eventBus.Subscribe<FixedUpdateEvent>(OnFixedUpdate);
        }

        private void OnCollisionEnter(CollisionEnterEvent e)
        {
            collisions.Add(e.collision);
        }

        private void OnCollisionStay(CollisionStayEvent e)
        {
            DoGroundCheck();
        }

        private void OnCollisionExit(CollisionExitEvent e)
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
            if (!playerState.groundState.Equals(GroundState.Grounded) && hasGroundedContact) landingRequested = true;
        }

        protected override void OnMovementInput(HorizontalInputEvent input)
        {
            if (isMovementDisabled) return;
            if (requestedMovement != 0f) return;
            requestedMovement = input.amount * 5f;
        }

        protected override void OnJumpInput(JumpInputEvent e)
        {
            if (isJumpingDisabled) return;
            if (jumpRequested) return;
            if (!playerState.groundState.Equals(GroundState.Grounded)) return;
            if (0f < jumpCooldown) return;
            jumpRequested = true;

        }

        private void OnFixedUpdate(FixedUpdateEvent e)
        {
            if (jumpRequested) performJump();
            if (landingRequested) PerformLanding();
            if (requestedMovement != 0f) performMovement();
            else performBreak();
        }

        private void performJump()
        {
            Debug.Log("JUMP");
            Vector2 impulseVector = new Vector2(0, 12f - rb2d.velocity.y);
            rb2d.AddForce(impulseVector, ForceMode2D.Impulse);
            playerState.groundState = GroundState.Airborne;
            jumpCooldown = maxJumpCooldown;
            eventBus.Subscribe<UpdateEvent>(ReduceJumpCooldown);
            jumpRequested = false;
        }

        private void performMovement()
        {
            requestedMovement *= 3f;
            float forceToApply;
            float velocityDirection = Mathf.Sign(rb2d.velocity.x * requestedMovement);
            if (0f < velocityDirection)
            {
                float lerpAmount = Mathf.Abs(rb2d.velocity.x) / maxHorizonalVelocity;
                forceToApply = velocityDirection < 0f ? requestedMovement : Mathf.Lerp(requestedMovement, 0f, lerpAmount);
            }
            else
            {
                forceToApply = requestedMovement;
            }
            rb2d.AddForce(forceToApply * Vector2.right);
            if (Mathf.Sign(requestedMovement * rb2d.velocity.x) < 0) performBreak();
            eventBus.Publish(new HorizontalMovementEvent(requestedMovement));
            requestedMovement = 0f;
        }

        private void performBreak()
        {
            float breakModifier = playerState.groundState == GroundState.Airborne ? 1f : 3f;
            float breakForce = -rb2d.velocity.x * breakModifier;
            rb2d.AddForce(Vector2.right * breakForce, ForceMode2D.Force);
        }

        private void PerformLanding()
        {
            Debug.Log("Perform landing");
            playerState.groundState = GroundState.Grounded;
            landingRequested = false;
            if (0 < requestedMovement * rb2d.velocity.x) return;
            rb2d.AddForce(Vector2.right * -rb2d.velocity.x, ForceMode2D.Impulse);
        }

        private void ReduceJumpCooldown(UpdateEvent e)
        {
            jumpCooldown -= Time.deltaTime;
            if (jumpCooldown <= 0f) eventBus.Unsubscribe<UpdateEvent>(ReduceJumpCooldown);
        }
    }
}