using UnityEngine;

namespace PlayerSystem
{
    public class Tight2DMovement : PlayerMovement
    {
        MonoBehaviour mb;
        public Rigidbody2D rb2d;

        private PlayerState playerState;

        public Tight2DMovement(EventBus eventBus, PlayerMovementScriptable movementValues, MonoBehaviour mb, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler groundTrigger) : base(eventBus, movementValues)
        {
            this.mb = mb;
            this.playerState = playerState;
            this.rb2d = rb2d;
            SetGroundCallbacks(groundTrigger);

            eventBus.Subscribe<HorizontalInputEvent>(MoveHorizontally);
            eventBus.Subscribe<JumpInputEvent>(Jump);
            eventBus.Subscribe<UpdateEvent>(UpdateGravity);
            eventBus.Subscribe<UseSquarePowerEvent>(onSquarePowerToggle);
        }

        public override void Jump(JumpInputEvent input)
        {
            if (playerState.groundState != GroundState.Grounded) return;
            if (playerState.healthState == HealthState.Stagger) return;
            if (isJumpingBlocked) return;
            rb2d.velocity = new Vector2(rb2d.velocity.x, movementValues.jumpVelocity);
            playerState.groundState = GroundState.Airborne;
            eventBus.Publish(new JumpMovementEvent());
        }

        public override void MoveHorizontally(HorizontalInputEvent input)
        {
            if (playerState.healthState == HealthState.Stagger) return;
            if (isMovementBlocked) return;
            rb2d.velocity = new Vector2(input.amount * movementValues.horizontalVelocity, rb2d.velocity.y);
            eventBus.Publish(new HorizontalMovementEvent(input.amount));
        }

        private void SetGroundCallbacks(TriggerEventHandler groundTrigger)
        {
            groundTrigger.OnTriggerEnter2DAction.AddListener((other) =>
            {
                if (other.gameObject.tag == "Ground")
                {
                    playerState.groundState = GroundState.Grounded;
                    eventBus.Publish(new GroundedMovementEvent());
                }
            });

            groundTrigger.OnTriggerExit2DAction.AddListener((other) =>
            {
                if (other.gameObject.tag == "Ground")
                {
                    playerState.groundState = GroundState.Airborne;
                    eventBus.Publish(new UngroundedMovementEvent());
                }
            });
        }

        public override void UpdateGravity(UpdateEvent e)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y - movementValues.gravity * Time.deltaTime);
        }

        private void onSquarePowerToggle(UseSquarePowerEvent e)
        {
            isMovementBlocked = isJumpingBlocked = e.toggle;
        }
    }
}
