using System;
using UnityEngine;

namespace PlayerSystem
{
    public class CirclePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;

        private readonly float maxPowerDuration = 0.5f;
        private float currentPowerTime = 0f;
        private float cooldownTimeLeft = 0f;

        public CirclePowerModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler leftTrigger, TriggerEventHandler rightTrigger)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;

            eventBus.Subscribe<CirclePowerInputEvent>(togglePower);
        }

        private void togglePower(CirclePowerInputEvent e)
        {
            if (cooldownTimeLeft > 0f) return;
            int facingDirectionInt = playerState.facingDirection == Direction.Right ? 1 : -1;
            rb2d.velocity = new Vector2(10f * facingDirectionInt, 0f);
            currentPowerTime = 0f;
            cooldownTimeLeft = 1f;
            eventBus.Subscribe<UpdateEvent>(timeoutPower);
            eventBus.Subscribe<UpdateEvent>(reducePowerCooldown);
            eventBus.Publish(new ToggleCirclePowerEvent(true, playerState.facingDirection));
        }

        private void timeoutPower(UpdateEvent e)
        {
            currentPowerTime += Time.deltaTime;

            if (currentPowerTime < maxPowerDuration) return;
            eventBus.Unsubscribe<UpdateEvent>(timeoutPower);
            eventBus.Publish(new ToggleCirclePowerEvent(false, playerState.facingDirection));
        }

        private void reducePowerCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft > 0f) return;
            eventBus.Unsubscribe<UpdateEvent>(reducePowerCooldown);
        }
    }
}