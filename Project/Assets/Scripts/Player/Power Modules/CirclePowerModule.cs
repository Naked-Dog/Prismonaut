using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerSystem
{
    public class CirclePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private TriggerEventHandler leftTrigger;
        private TriggerEventHandler rightTrigger;

        private readonly float powerDuration = 0.5f;
        private readonly float cooldownDuration = 1f;
        private float powerTimeLeft = 0f;
        private float cooldownTimeLeft = 0f;

        public CirclePowerModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler leftTrigger, TriggerEventHandler rightTrigger)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.leftTrigger = leftTrigger;
            this.rightTrigger = rightTrigger;

            eventBus.Subscribe<CirclePowerInputEvent>(activate);
        }

        private void activate(CirclePowerInputEvent e)
        {
            if (cooldownTimeLeft > 0f) return;

            int facingDirectionInt = playerState.facingDirection == Direction.Right ? 1 : -1;
            rb2d.velocity = new Vector2(10f * facingDirectionInt, 0f);
            powerTimeLeft = powerDuration;

            TriggerEventHandler triggerToActivate = playerState.facingDirection == Direction.Right ? rightTrigger : leftTrigger;
            triggerToActivate.OnTriggerEnter2DAction.AddListener(onTriggerEnter);

            eventBus.Subscribe<UpdateEvent>(reduceTimeLeft);
            eventBus.Subscribe<UpdateEvent>(deactivateOnStandstill);
            eventBus.Publish(new ToggleCirclePowerEvent(true));
        }

        private void reduceTimeLeft(UpdateEvent e)
        {
            powerTimeLeft -= Time.deltaTime;
            if (0 < powerTimeLeft) return;
            deactivate();
        }

        private void deactivate()
        {
            leftTrigger.OnTriggerEnter2DAction.RemoveListener(onTriggerEnter);
            rightTrigger.OnTriggerEnter2DAction.RemoveListener(onTriggerEnter);

            cooldownTimeLeft = cooldownDuration;
            eventBus.Subscribe<UpdateEvent>(reduceCooldown);

            eventBus.Unsubscribe<UpdateEvent>(reduceTimeLeft);
            eventBus.Unsubscribe<UpdateEvent>(deactivateOnStandstill);
            eventBus.Publish(new ToggleCirclePowerEvent(false));
        }

        private void reduceCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft > 0f) return;
            eventBus.Unsubscribe<UpdateEvent>(reduceCooldown);
        }

        private void deactivateOnStandstill(UpdateEvent e)
        {
            if (0.1f < Mathf.Abs(rb2d.velocity.x)) return;
            deactivate();
        }


        private void onTriggerEnter(Collider2D other)
        {
            if (other.CompareTag("Breakable"))
            {
                GameObject.Destroy(other.gameObject);
            }

            // Todo: Place enemy collision logic here
        }
    }
}