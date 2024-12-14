using UnityEngine;

namespace PlayerSystem
{
    public class SquarePowerModule
    {
        private EventBus eventBus;
        private Rigidbody2D rb2d;

        public SquarePowerModule(EventBus eventBus, Rigidbody2D rb2d, TriggerEventHandler groundTrigger)
        {
            this.eventBus = eventBus;
            this.rb2d = rb2d;
        }

        public void togglePower(bool toggle)
        {

            if (toggle) rb2d.velocity = new Vector2(0, -10f);
            eventBus.Publish(new ToggleSquarePowerEvent(toggle));
        }
    }
}