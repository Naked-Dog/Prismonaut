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
        private bool isGrounded = false;
        private bool jumpRequested = false;
        private float requestedMovement = 0f;

        readonly float jumpCooldown = 0.2f;

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

        private void setIsGrounded(bool isGrounded)
        {
            if (!this.isGrounded && isGrounded) Debug.Log("GROUNDED");
            this.isGrounded = isGrounded;
            playerState.groundState = isGrounded ? GroundState.Grounded : GroundState.Airborne;
        }

        private void OnCollisionEnter(CollisionEnterEvent e)
        {
            collisions.Add(e.collision);
        }

        private void OnCollisionStay(CollisionStayEvent e)
        {
            if (!collisions.Contains(e.collision)) Debug.Log("Collision not found");
            DoGroundCheck();
        }

        private void OnCollisionExit(CollisionExitEvent e)
        {
            collisions.Remove(e.collision);
            DoGroundCheck();
        }

        private void DoGroundCheck()
        {
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
            setIsGrounded(hasGroundedContact);
        }

        protected override void OnMovementInput(HorizontalInputEvent input)
        {
            if (isMovementDisabled) return;
            if (requestedMovement != 0f) return;
            requestedMovement = input.amount * 5f;
        }

        protected override void OnJumpInput(JumpInputEvent e)
        {
            if (!isGrounded) return;
            jumpRequested = true;
        }

        private void OnFixedUpdate(FixedUpdateEvent e)
        {
            if (jumpRequested) performJump();
            if (requestedMovement != 0f) performMovement();
            else performBreak();
        }

        private void performJump()
        {
            Vector2 impulseVector = new Vector2(0, 10f);
            float clampedYVelocity = Mathf.Max(rb2d.velocity.y, 0);
            rb2d.velocity = new Vector2(rb2d.velocity.x, clampedYVelocity);
            rb2d.AddForce(impulseVector, ForceMode2D.Impulse);
            jumpRequested = false;
        }

        private void performMovement()
        {
            rb2d.AddForce(requestedMovement * Vector2.right);
            eventBus.Publish(new HorizontalMovementEvent(requestedMovement));
            requestedMovement = 0f;
        }

        private void performBreak()
        {
            float modificator = playerState.groundState == GroundState.Airborne ? 0f : 1.5f;
            rb2d.velocity = Vector2.Lerp(rb2d.velocity, new Vector2(0, rb2d.velocity.y), 0.1f);
        }
    }
}