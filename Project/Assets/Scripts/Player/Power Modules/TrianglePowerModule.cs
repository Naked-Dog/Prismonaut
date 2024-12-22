using UnityEngine;

namespace PlayerSystem
{
    public class TrianglePowerModule
    {
        private EventBus eventBus;
        private Rigidbody2D rb2d;

        private readonly float maxPowerDuration = 0.5f;
        private float currentPowerTime = 0f;
        private float cooldownTimeLeft = 0f;

        public TrianglePowerModule(EventBus eventBus, Rigidbody2D rb2d, TriggerEventHandler upTrigger)
        {
            this.eventBus = eventBus;
            this.rb2d = rb2d;

            eventBus.Subscribe<TrianglePowerInputEvent>(togglePower);
        }

        private void togglePower(TrianglePowerInputEvent e)
        {
            if (cooldownTimeLeft > 0f) return;
            rb2d.velocity = new Vector2(0, 10f);
            cooldownTimeLeft = 1f;
            currentPowerTime = 0f;
            eventBus.Subscribe<UpdateEvent>(timeoutPower);
            eventBus.Subscribe<UpdateEvent>(reducePowerCooldown);
            eventBus.Publish(new ToggleTrianglePowerEvent(true));
        }

        private void timeoutPower(UpdateEvent e)
        {
            currentPowerTime += Time.deltaTime;
            if (currentPowerTime < maxPowerDuration) return;
            eventBus.Unsubscribe<UpdateEvent>(timeoutPower);
            eventBus.Publish(new ToggleTrianglePowerEvent(false));
        }

        private void reducePowerCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft > 0f) return;
            eventBus.Unsubscribe<UpdateEvent>(reducePowerCooldown);
        }
    }
}