using UnityEngine;

namespace PlayerSystem
{
    public class SquarePowerModule
    {
        private EventBus eventBus;

        public SquarePowerModule(EventBus eventBus)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe<UseSquarePowerEvent>(usePower);
        }

        private void usePower(UseSquarePowerEvent e)
        {
            Debug.Log("Square Power");
        }
    }
}