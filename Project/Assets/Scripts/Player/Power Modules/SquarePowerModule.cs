using UnityEngine;

namespace PlayerSystem
{
    public class SquarePowerModule
    {
        private EventBus eventBus;
        private Rigidbody2D rb2d;
        private bool isPowerActive = false;

        public SquarePowerModule(EventBus eventBus, Rigidbody2D rb2d)
        {
            this.eventBus = eventBus;
            this.rb2d = rb2d;
        }

        public void togglePower(bool toggle)
        {
            if (isPowerActive == toggle) return;

            if (toggle)
            {
                rb2d.velocity = new Vector2(0, -10f);
                eventBus.Publish(new UseSquarePowerEvent(true));
            }
            else
            {
                eventBus.Publish(new UseSquarePowerEvent(false));
            }

            isPowerActive = toggle;
        }
    }
}