using UnityEngine;

namespace PlayerSystem
{
    public class CirclePowerModule
    {
        private EventBus eventBus;

        public CirclePowerModule(EventBus eventBus)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe<UseCirclePowerEvent>(usePower);
        }

        private void usePower(UseCirclePowerEvent e)
        {
            Debug.Log("Circle Power");
        }
    }
}