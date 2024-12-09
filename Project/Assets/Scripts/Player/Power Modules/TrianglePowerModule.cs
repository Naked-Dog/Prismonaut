using UnityEngine;

namespace PlayerSystem
{
    public class TrianglePowerModule
    {
        private EventBus eventBus;

        public TrianglePowerModule(EventBus eventBus)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe<UseTrianglePowerEvent>(usePower);
        }

        private void usePower(UseTrianglePowerEvent e)
        {
            Debug.Log("Triangle Power");
        }
    }
}