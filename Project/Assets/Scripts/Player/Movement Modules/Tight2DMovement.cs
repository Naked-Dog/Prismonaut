using System.Collections;
using UnityEngine;

namespace PlayerSystem
{
    public class Tight2DMovement : PlayerMovement
    {
        MonoBehaviour mb;
        public Rigidbody2D rb2d;

        private PlayerState playerState;
        private Collider2D coll;

        private bool isJumping = false;
        private bool isFalling = false;
        private float jumpTimer;
        private RaycastHit2D groundHit;
        private float gravity;
        private bool isTriangleActive = false;
        private bool isCircleActive = false;


        public Tight2DMovement(EventBus eventBus, PlayerState playerState, PlayerMovementScriptable movementValues, Rigidbody2D rb2d, TriggerEventHandler groundTrigger, MonoBehaviour mb) : base(eventBus, movementValues)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.mb = mb;
            gravity = movementValues.gravity;
            coll = rb2d.GetComponent<Collider2D>();
            SetGroundCallbacks(groundTrigger);

            eventBus.Subscribe<HorizontalInputEvent>(MoveHorizontally);
            eventBus.Subscribe<JumpInputEvent>(Jump);
            //eventBus.Subscribe<UpdateEvent>(UpdateGravity);
            eventBus.Subscribe<ToggleSquarePowerEvent>(onSquarePowerToggle);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(onTrianglePowerToggle);
            eventBus.Subscribe<ToggleCirclePowerEvent>(onCirclePowerToggle);
            eventBus.Subscribe<PauseEvent>(OnPause);
            eventBus.Subscribe<PauseInputEvent>(OnInputPause);
        }

        public override void Jump(JumpInputEvent input)
        {
            //if (playerState.groundState != GroundState.Grounded) return;
            if (playerState.activePower != Power.None) return;
            if (playerState.healthState == HealthState.Stagger) return;
            if (isJumpingDisabled) return;

            if (input.jumpInputAction.WasPressedThisFrame() && IsGrounded())
            {
                isJumping = true;
                jumpTimer = movementValues.jumpTime;
                rb2d.velocity = new Vector2(rb2d.velocity.x, movementValues.jumpForce);
                playerState.groundState = GroundState.Airborne;
                eventBus.Publish(new JumpMovementEvent());
            }

            if (input.jumpInputAction.IsPressed())
            {
                if (isJumping && jumpTimer > 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, movementValues.jumpForce);
                    jumpTimer -= Time.deltaTime;
                }
                else if (jumpTimer <= 0)
                {
                    isFalling = true;
                    isJumping = false;
                    return;
                }
                else
                {
                    isJumping = false;
                }
            }

            if (input.jumpInputAction.WasReleasedThisFrame())
            {
                isFalling = true;
                isJumping = false;
            }

            if (!isJumping && CheckForLand())
            {
                //Landing animation
                Debug.Log("Landing");
            }

            DrawGroundCheck();
        }

        public override void MoveHorizontally(HorizontalInputEvent input)
        {
            if (playerState.healthState == HealthState.Stagger) return;
            if (isMovementDisabled) return;

            float velocityX = playerState.groundState == GroundState.Airborne ? movementValues.horizontalVelocity * 0.6f : movementValues.horizontalVelocity;
            rb2d.velocity = new Vector2(input.amount * velocityX, rb2d.velocity.y);
            eventBus.Publish(new HorizontalMovementEvent(input.amount));
        }

        private void SetGroundCallbacks(TriggerEventHandler groundTrigger)
        {
            groundTrigger.OnTriggerEnter2DAction.AddListener((other) =>
            {
                if (other.gameObject.tag == "Ground" || other.gameObject.CompareTag("Platform"))
                {
                    if (other.gameObject.CompareTag("Platform"))
                    {
                        other.gameObject.GetComponent<IPlatform>()?.PlatformEnterAction(playerState, rb2d);
                    }

                    mb.StartCoroutine(JumpEnd());
                    //playerState.groundState = GroundState.Grounded;
                    IsGrounded();
                    eventBus.Publish(new GroundedMovementEvent());

                }
            });

            groundTrigger.OnTriggerExit2DAction.AddListener((other) =>
            {
                if (other.gameObject.tag == "Ground" || other.gameObject.CompareTag("Platform"))
                {
                    if (other.gameObject.CompareTag("Platform"))
                    {
                        other.gameObject.GetComponent<IPlatform>()?.PlatformExitAction(rb2d);
                    }
                    else
                    {
                        IsGrounded();
                        SaveSafeGround();
                    }
                    //playerState.groundState = GroundState.Airborne;
                    eventBus.Publish(new UngroundedMovementEvent());
                }
            });
        }

        public override void UpdateGravity(UpdateEvent e)
        {
            if (isGravityDisabled) return;
            //rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y - movementValues.gravity * Time.deltaTime);
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
                if (!isCircleActive) rb2d.gravityScale = gravity;
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
                if (!isTriangleActive) rb2d.gravityScale = gravity;
            }
            isMovementDisabled = isJumpingDisabled = isGravityDisabled = isCircleActive = e.toggle;
        }

        private void OnPause(PauseEvent e)
        {
            if (!playerState.isPaused)
            {
                playerState.velocity = rb2d.velocity;
                rb2d.velocity = Vector2.zero;
            }
            else
            {
                rb2d.velocity = playerState.velocity;
            }
            playerState.isPaused = !playerState.isPaused;
        }

        private void OnInputPause(PauseInputEvent e)
        {
            OnPause(new PauseEvent());
        }

        private bool IsGrounded()
        {
            groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, movementValues.groundCheckExtraHeight, movementValues.groundLayerMask);
            if (groundHit.collider != null)
            {
                playerState.groundState = GroundState.Grounded;
                return true;
            }
            else
            {
                playerState.groundState = GroundState.Airborne;
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
            float modificator = playerState.groundState == GroundState.Airborne ? 0f : 1f;
            float direction = 1;
            if (playerState.facingDirection == Direction.Left) direction = -1;
            playerState.lastSafeGroundLocation = new Vector2(rb2d.position.x - (modificator * direction), rb2d.position.y);
        }

        private IEnumerator JumpEnd()
        {
            isMovementDisabled = true;
            isJumpingDisabled = true;
            rb2d.velocity = Vector2.zero;

            yield return new WaitForSeconds(0.2f);

            isMovementDisabled = false;
            isJumpingDisabled = false;
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
            Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + movementValues.groundCheckExtraHeight), rayColor);
            Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + movementValues.groundCheckExtraHeight), rayColor);
            Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + movementValues.groundCheckExtraHeight), Vector2.right * (coll.bounds.extents.x * 2), rayColor);
        }

        #endregion
    }
}
