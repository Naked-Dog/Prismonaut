using UnityEngine;

namespace PlayerSystem
{
    public class CirclePowerModule
    {
        private EventBus eventBus;
        private Rigidbody2D rb2d;

        public CirclePowerModule(EventBus eventBus, Rigidbody2D rb2d, TriggerEventHandler leftTrigger, TriggerEventHandler rightTrigger)
        {
            this.eventBus = eventBus;
            this.rb2d = rb2d;
            eventBus.Subscribe<ToggleCirclePowerEvent>(togglePower);
        }

        private void togglePower(ToggleCirclePowerEvent e)
        {
            rb2d.velocity = new Vector2(0, -10f);
            eventBus.Publish(new ToggleSquarePowerEvent(true));
        }
    }
}