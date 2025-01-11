using System.Collections;
using TMPro;
using UnityEngine;

namespace PlayerSystem
{
    public class Tight2DMovement : PlayerMovement
    {
        MonoBehaviour mb;
        public Rigidbody2D rb2d;

        private PlayerState playerState;
        private Collider2D coll;

        private bool isFalling = false;
        private float jumpTimer;
        private RaycastHit2D groundHit;
        private bool isTriangleActive = false;
        private bool isCircleActive = false;
        private bool isLanding = false;
        private PlayerAudioModule playerAudio;

        public Tight2DMovement(EventBus eventBus, PlayerState playerState, PlayerMovementScriptable movementValues, Rigidbody2D rb2d, TriggerEventHandler groundTrigger, PlayerAudioModule playerAudio, MonoBehaviour mb) : base(eventBus, movementValues)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.playerAudio = playerAudio;
            this.mb = mb;
            coll = rb2d.GetComponent<Collider2D>();
            SetGroundCallbacks(groundTrigger);

            eventBus.Subscribe<HorizontalInputEvent>(MoveHorizontally);
            eventBus.Subscribe<JumpInputEvent>(Jump);
            eventBus.Subscribe<ToggleSquarePowerEvent>(onSquarePowerToggle);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(onTrianglePowerToggle);
            eventBus.Subscribe<ToggleCirclePowerEvent>(onCirclePowerToggle);
            eventBus.Subscribe<PauseEvent>(StopPlayerMovement);
            eventBus.Subscribe<UnpauseEvent>(EnablePlayerMovement);
        }

        public override void Jump(JumpInputEvent input)
        {
            if (playerState.activePower != Power.None) return;
            if (playerState.healthState == HealthState.Stagger) return;
            if (isJumpingDisabled) return;

            if (input.jumpInputAction.WasPressedThisFrame() && IsGrounded() && !isFalling)
            {
                jumpTimer = movementValues.jumpTime;
                rb2d.velocity = new Vector2(rb2d.velocity.x, movementValues.jumpForce);
                SaveSafeGround();
                playerState.groundState = GroundState.Airborne;
                eventBus.Publish(new JumpMovementEvent());
            }

            if (input.jumpInputAction.IsPressed() && !isFalling)
            {
                if (playerState.groundState == GroundState.Airborne && jumpTimer > 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, movementValues.jumpForce);
                    jumpTimer -= Time.deltaTime;
                }
                else if (jumpTimer <= 0)
                {
                    isFalling = true;
                    return;
                }
            }

            if (input.jumpInputAction.WasReleasedThisFrame())
            {
                isFalling = true;
            }

            if (playerState.groundState == GroundState.Grounded && CheckForLand())
            {
                //Landing animation
            }

            //DrawGroundCheck();
        }

        public override void MoveHorizontally(HorizontalInputEvent input)
        {
            if (playerState.healthState == HealthState.Stagger) return;
            if (isMovementDisabled) return;

            rb2d.velocity = new Vector2(input.amount * movementValues.horizontalVelocity, rb2d.velocity.y);
            eventBus.Publish(new HorizontalMovementEvent(input.amount));
        }

        private void SetGroundCallbacks(TriggerEventHandler groundTrigger)
        {
            groundTrigger.OnTriggerEnter2DAction.AddListener((other) =>
            {
                if (other.gameObject.CompareTag("Platform") || other.gameObject.layer == 6)
                {
                    if (other.gameObject.CompareTag("Platform"))
                    {
                        other.gameObject.GetComponent<IPlatform>()?.PlatformEnterAction(playerState, rb2d);
                    }

                    //mb.StartCoroutine(JumpEnd());
                    playerState.groundState = GroundState.Grounded;
                    //IsGrounded();
                    eventBus.Publish(new GroundedMovementEvent());

                }
            });

            groundTrigger.OnTriggerExit2DAction.AddListener((other) =>
            {
                if (other.gameObject.layer == 6 || other.gameObject.CompareTag("Platform"))
                {
                    if (other.gameObject.CompareTag("Platform"))
                    {
                        other.gameObject.GetComponent<IPlatform>()?.PlatformExitAction(rb2d);
                    }
                    if (other.gameObject.CompareTag("Ground"))
                    {
                        SaveSafeGround();
                    }
                    eventBus.Publish(new UngroundedMovementEvent());
                }
            });
        }

        private void onSquarePowerToggle(ToggleSquarePowerEvent e)
        {
            isMovementDisabled = isJumpingDisabled = e.toggle;
        }

        private void onTrianglePowerToggle(ToggleTrianglePowerEvent e)
        {
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

        private void StopPlayerMovement(PauseEvent e)
        {
            playerState.velocity = rb2d.velocity;
            rb2d.velocity = Vector2.zero;
            rb2d.gravityScale = 0;
            playerState.isPaused = true;
            eventBus.Publish(new StopPlayerInputsEvent());
        }

        private void EnablePlayerMovement(UnpauseEvent e)
        {
            rb2d.velocity = playerState.velocity;
            rb2d.gravityScale = movementValues.gravity;
            playerState.isPaused = false;
            eventBus.Publish(new EnablePlayerInputsEvent());
        }

        private bool IsGrounded()
        {
            groundHit = Physics2D.BoxCast(coll.bounds.center - new Vector3(0, coll.bounds.extents.y), new Vector2(coll.bounds.size.x, coll.bounds.size.y * 0.1f), 0f, Vector2.down, movementValues.groundCheckExtraHeight, movementValues.groundLayerMask);
            if (groundHit.collider != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckForLand()
        {
            if (isFalling)
            {
                if (IsGrounded())
                {
                    isFalling = false;
                    return true;
                }
                else
                {
                    isFalling = true;
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
            float modificator = playerState.groundState == GroundState.Airborne ? 0f : 2f;
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
