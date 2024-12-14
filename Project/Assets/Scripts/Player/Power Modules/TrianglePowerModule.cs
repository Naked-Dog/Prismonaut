using UnityEngine;

namespace PlayerSystem
{
    public class TrianglePowerModule
    {
        private EventBus eventBus;

        public TrianglePowerModule(EventBus eventBus, TriggerEventHandler upTrigger)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe<ToggleTrianglePowerEvent>(usePower);
        }

        private void usePower(ToggleTrianglePowerEvent e)
        {
            Debug.Log("Triangle Power");
        }
    }
}