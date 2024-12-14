using UnityEngine;

namespace PlayerSystem
{
    public class CirclePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;

        public CirclePowerModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler leftTrigger, TriggerEventHandler rightTrigger)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;

            eventBus.Subscribe<CirclePowerInputEvent>(togglePower);
        }

        private void togglePower(CirclePowerInputEvent e)
        {
            int facingDirectionInt = playerState.facingDirection == Direction.Right ? 1 : -1;
            rb2d.velocity = new Vector2(10f * facingDirectionInt, 0f);
            eventBus.Publish(new ToggleCirclePowerEvent(true, playerState.facingDirection));
        }
    }
}