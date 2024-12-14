using UnityEngine;

namespace PlayerSystem
{
    public class TrianglePowerModule
    {
        private EventBus eventBus;
        private Rigidbody2D rb2d;

        public TrianglePowerModule(EventBus eventBus, Rigidbody2D rb2d, TriggerEventHandler upTrigger)
        {
            this.eventBus = eventBus;
            this.rb2d = rb2d;

            eventBus.Subscribe<TrianglePowerInputEvent>(togglePower);
        }

        private void togglePower(TrianglePowerInputEvent e)
        {
            Debug.Log("Triangle Power");
            eventBus.Publish(new ToggleTrianglePowerEvent(true));
        }
    }
}