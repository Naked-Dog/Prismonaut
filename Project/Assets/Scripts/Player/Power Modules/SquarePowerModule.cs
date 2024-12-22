using Unity.VisualScripting;
using UnityEngine;

namespace PlayerSystem
{
    public class SquarePowerModule
    {
        private EventBus eventBus;
        private Rigidbody2D rb2d;

        private readonly float cooldownDuration = 0.5f;
        private float cooldownTimeLeft = 0f;
        private bool isActive = false;

        public SquarePowerModule(EventBus eventBus, Rigidbody2D rb2d, TriggerEventHandler groundTrigger)
        {
            this.eventBus = eventBus;
            this.rb2d = rb2d;

            eventBus.Subscribe<SquarePowerInputEvent>(togglePower);
        }

        private void togglePower(SquarePowerInputEvent e)
        {
            if (e.toggle == isActive) return;
            if (e.toggle) activate();
            else deactivate();
        }

        private void activate()
        {
            if (0f < cooldownTimeLeft) return;

            isActive = true;
            rb2d.velocity = new Vector2(0, -10f);
            eventBus.Publish(new ToggleSquarePowerEvent(true));
        }

        private void reduceCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (0 < cooldownTimeLeft) return;

            eventBus.Unsubscribe<UpdateEvent>(reduceCooldown);
        }

        private void deactivate()
        {
            isActive = false;
            cooldownTimeLeft = cooldownDuration;
            eventBus.Subscribe<UpdateEvent>(reduceCooldown);
            eventBus.Publish(new ToggleSquarePowerEvent(false));
        }
    }
}