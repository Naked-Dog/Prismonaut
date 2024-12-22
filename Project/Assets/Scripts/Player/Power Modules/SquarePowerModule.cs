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

            eventBus.Subscribe<SquarePowerInputEvent>(togglePower);
        }

        public void togglePower(SquarePowerInputEvent e)
        {
            if (e.toggle) rb2d.velocity = new Vector2(0, -10f);
            eventBus.Publish(new ToggleSquarePowerEvent(e.toggle));
        }
    }
}