using System.Collections;
using UnityEngine;

namespace PlayerSystem
{
    public class Tight2DMovement : PlayerMovement
    {
        MonoBehaviour mb;
        public Rigidbody2D rb2d;

        private PlayerState playerState;

        public Tight2DMovement(EventBus eventBus, PlayerState playerState, PlayerMovementScriptable movementValues, Rigidbody2D rb2d, TriggerEventHandler groundTrigger, MonoBehaviour mb) : base(eventBus, movementValues)
        {
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.mb = mb;
            SetGroundCallbacks(groundTrigger);

            eventBus.Subscribe<HorizontalInputEvent>(MoveHorizontally);
            eventBus.Subscribe<JumpInputEvent>(Jump);
            eventBus.Subscribe<UpdateEvent>(UpdateGravity);
            eventBus.Subscribe<ToggleSquarePowerEvent>(onSquarePowerToggle);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(onTrianglePowerToggle);
            eventBus.Subscribe<ToggleCirclePowerEvent>(onCirclePowerToggle);
            eventBus.Subscribe<PauseEvent>(OnPause);
            eventBus.Subscribe<PauseInputEvent>(OnInputPause);
        }

        public override void Jump(JumpInputEvent input)
        {
            if (playerState.groundState != GroundState.Grounded) return;
            if (playerState.healthState == HealthState.Stagger) return;
            if (isJumpingDisabled) return;
            rb2d.velocity = new Vector2(rb2d.velocity.x, movementValues.jumpVelocity);
            playerState.groundState = GroundState.Airborne;
            eventBus.Publish(new JumpMovementEvent());
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

                    playerState.groundState = GroundState.Grounded;
                    mb.StartCoroutine(JumpEnd());
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
                        SaveSafeGround();
                    }
                    playerState.groundState = GroundState.Airborne;
                    eventBus.Publish(new UngroundedMovementEvent());
                }
            });
        }

        public override void UpdateGravity(UpdateEvent e)
        {
            if (isGravityDisabled) return;
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y - movementValues.gravity * Time.deltaTime);
        }

        private void onSquarePowerToggle(ToggleSquarePowerEvent e)
        {
            isMovementDisabled = isJumpingDisabled = isGravityDisabled = e.toggle;
        }

        private void onTrianglePowerToggle(ToggleTrianglePowerEvent e)
        {
            isMovementDisabled = isJumpingDisabled = e.toggle;
        }

        private void onCirclePowerToggle(ToggleCirclePowerEvent e)
        {
            isMovementDisabled = isJumpingDisabled = isGravityDisabled = e.toggle;
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

        private void SaveSafeGround()
        {
            float modificator = playerState.groundState == GroundState.Airborne ? 0f : 1f;
            playerState.lastSafeGroundLocation = new Vector2(rb2d.position.x - modificator, rb2d.position.y);
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
    }
}
