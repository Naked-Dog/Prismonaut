using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using CameraSystem;
using Unity.VisualScripting;

namespace PlayerSystem
{
    public class Tight2DMovement : PlayerMovement
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
        private float jumpTimer;
        private RaycastHit2D groundHit;
        private bool isTriangleActive = false;
        private bool isCircleActive = false;
        private bool isLanding = false;
        private CameraState cameraState;
        private PlayerAudioModule playerAudio;
        private bool isLookingDown = false;

        public Tight2DMovement(EventBus eventBus, PlayerState playerState, PlayerMovementScriptable movementValues, Rigidbody2D rb2d, TriggerEventHandler groundTrigger, CameraState cameraState, PlayerAudioModule playerAudio, MonoBehaviour mb) : base(eventBus, movementValues)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.playerAudio = playerAudio;
            this.mb = mb;
            this.cameraState = cameraState;
            coll = rb2d.GetComponent<Collider2D>();
            eventBus.Subscribe<OnUpdate>(CheckForVelocity);
            eventBus.Subscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Subscribe<OnJumpInput>(OnJumpInput);
            eventBus.Subscribe<ToggleSquarePowerEvent>(onSquarePowerToggle);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(onTrianglePowerToggle);
            eventBus.Subscribe<ToggleCirclePowerEvent>(onCirclePowerToggle);
            eventBus.Subscribe<RequestPause>(StopPlayerMovement);
            eventBus.Subscribe<RequestUnpause>(EnablePlayerMovement);
            eventBus.Subscribe<OnLookDownInput>(ToggleLookingDown);
            eventBus.Subscribe<OnCollisionEnter2D>(SetCollisionEnterCallbacks);
            eventBus.Subscribe<CollisionExit2D>(SetCollisionExitCallbacks);
        }

        protected override void OnJumpInput(OnJumpInput input)
        {
            if (playerState.activePower != Power.None) return;
            if (playerState.healthState == HealthState.Stagger) return;
            if (isJumpingDisabled) return;

            if (input.jumpInputAction.WasPressedThisFrame() && IsGrounded() && !IsFalling)
            {
                jumpTimer = movementValues.jumpTime;
                rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, movementValues.jumpForce);
                if (!IsPlatform()) SaveSafeGround();
                playerState.groundState = GroundState.Airborne;
                eventBus.Publish(new OnJumpMovement());
            }

            if (input.jumpInputAction.IsPressed() && !IsFalling)
            {
                if (playerState.groundState == GroundState.Airborne && jumpTimer > 0)
                {
                    rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, movementValues.jumpForce);
                    jumpTimer -= Time.deltaTime;
                }
                else if (jumpTimer <= 0)
                {
                    IsFalling = true;
                    return;
                }
            }

            if (input.jumpInputAction.WasReleasedThisFrame())
            {
                IsFalling = true;
            }

            if (playerState.groundState == GroundState.Grounded && CheckForLand())
            {
                //Landing animation
            }
        }

        private void CheckForVelocity(OnUpdate e)
        {
            float vSpeed = rb2d.linearVelocity.y > movementValues.maximumYSpeed ? movementValues.maximumYSpeed :
                rb2d.linearVelocity.y < movementValues.minimumYSpeed ? movementValues.minimumYSpeed : rb2d.linearVelocity.y;
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, vSpeed);
        }

        protected override void OnHorizontalInput(OnHorizontalInput input)
        {
            if (playerState.healthState == HealthState.Stagger) return;
            if (isMovementDisabled) return;


            rb2d.linearVelocity = new Vector2(input.amount * movementValues.horizontalVelocity, rb2d.linearVelocity.y);
            eventBus.Publish(new HorizontalMovementEvent(input.amount));

        }

        private void SetCollisionEnterCallbacks(OnCollisionEnter2D collisionEnterEvent)
        {
            collisionEnterEvent.collision.gameObject.GetComponent<IPlatform>()?.PlatformEnterAction(playerState, rb2d);
            if (collisionEnterEvent.collision.gameObject.layer == 6)
            {
                IsGrounded();
                eventBus.Publish(new OnGroundedMovement());
            }
        }

        private void SetCollisionExitCallbacks(CollisionExit2D collisionExitEvent)
        {
            collisionExitEvent.collision.gameObject.GetComponent<IPlatform>()?.PlatformExitAction(rb2d);
            if (collisionExitEvent.collision.gameObject.layer == 6)
            {
                if (collisionExitEvent.collision.gameObject.CompareTag("Ground")) SaveSafeGround();
                eventBus.Publish(new OnUngroundedMovement());
            }
        }

        private void onSquarePowerToggle(ToggleSquarePowerEvent e)
        {
            isMovementDisabled = isJumpingDisabled = e.toggle;
        }

        private void onTrianglePowerToggle(ToggleTrianglePowerEvent e)
        {
            cameraState.CameraPosState = CameraPositionState.Regular;
            if (e.toggle)
            {
                rb2d.gravityScale = 0;
            }
            else
            {
                if (!isCircleActive) rb2d.gravityScale = movementValues.gravity;
            }
            isMovementDisabled = isJumpingDisabled = isGravityDisabled = isTriangleActive = e.toggle;
        }

        private void onCirclePowerToggle(ToggleCirclePowerEvent e)
        {
            cameraState.CameraPosState = CameraPositionState.Regular;
            if (e.toggle)
            {
                rb2d.gravityScale = 0;
            }
            else
            {
                if (!isTriangleActive) rb2d.gravityScale = movementValues.gravity;
            }
            isMovementDisabled = isJumpingDisabled = isGravityDisabled = isCircleActive = e.toggle;
        }

        private void ToggleLookingDown(OnLookDownInput e)
        {
            if (e.toggle == isLookingDown) return;
            isLookingDown = e.toggle;
            if (isLookingDown && !_isFalling) cameraState.CameraPosState = CameraPositionState.LookingDown;
            else cameraState.CameraPosState = CameraPositionState.Regular;
        }

        private void StopPlayerMovement(RequestPause e)
        {
            playerState.velocity = rb2d.linearVelocity;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.gravityScale = 0;
            playerState.isPaused = true;
            eventBus.Publish(new RequestStopPlayerInputs());
        }

        private void EnablePlayerMovement(RequestUnpause e)
        {
            rb2d.linearVelocity = playerState.velocity;
            rb2d.gravityScale = movementValues.gravity;
            playerState.isPaused = false;
            eventBus.Publish(new RequestPlayerInputs());
        }

        private bool IsGrounded()
        {
            groundHit = Physics2D.BoxCast(coll.bounds.center - new Vector3(0, coll.bounds.extents.y), new Vector2(coll.bounds.size.x, coll.bounds.size.y * 0.1f), 0f, Vector2.down, movementValues.groundCheckExtraHeight, movementValues.groundLayerMask);
            if (groundHit.collider != null)
            {
                playerState.groundState = GroundState.Grounded;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPlatform()
        {
            groundHit = Physics2D.BoxCast(coll.bounds.center - new Vector3(0, coll.bounds.extents.y), new Vector2(coll.bounds.size.x, coll.bounds.size.y * 0.1f), 0f, Vector2.down, movementValues.groundCheckExtraHeight, movementValues.groundLayerMask);
            if (groundHit.collider != null)
            {
                return groundHit.collider.gameObject.CompareTag("Platform");
            }
            else
            {
                return false;
            }
        }

        private bool CheckForLand()
        {
            if (IsFalling)
            {
                if (IsGrounded())
                {
                    IsFalling = false;
                    return true;
                }
                else
                {
                    IsFalling = true;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void SaveSafeGround()
        {
            float modificator = playerState.groundState == GroundState.Airborne ? 0f : 1.5f;
            float direction = 1;
            if (playerState.facingDirection == Direction.Left) direction = -1;
            playerState.lastSafeGroundLocation = new Vector2(rb2d.position.x - (modificator * direction), rb2d.position.y);
        }

        private IEnumerator JumpEnd()
        {
            isJumpingDisabled = true;
            isLanding = true;

            yield return new WaitForSeconds(0.05f);

            isJumpingDisabled = false;
            isLanding = false;
        }

        #region Debug Functions
        private void DrawGroundCheck()
        {
            Color rayColor;
            if (IsGrounded())
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
            Debug.DrawRay(coll.bounds.center/*  - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y) */, Vector2.right * (coll.bounds.extents.x * 2), Color.blue);
            Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, coll.bounds.extents.y), Vector2.down * (coll.bounds.extents.y + movementValues.groundCheckExtraHeight), Color.yellow);
            Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y), Vector2.down * (coll.bounds.extents.y + movementValues.groundCheckExtraHeight), rayColor);
            Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + movementValues.groundCheckExtraHeight), Vector2.right * (coll.bounds.extents.x * 2), rayColor);
        }
        #endregion
    }
}
