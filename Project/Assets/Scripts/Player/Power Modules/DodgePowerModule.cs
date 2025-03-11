using UnityEngine;

namespace PlayerSystem
{
    public class DodgePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;

        private PlayerMovementScriptable movementValues;

        private float powerTimeLeft = 0f;
        private float cooldownTimeLeft = 0f;

        private Vector2 inputDirection = Vector2.zero;

        public DodgePowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PlayerMovementScriptable movementValues)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.movementValues = movementValues;

            eventBus.Subscribe<CirclePowerInputEvent>(activate);
            eventBus.Subscribe<HorizontalInputEvent>(OnHorizontaInput);
            eventBus.Subscribe<VerticalInputEvent>(OnVerticalInput);
        }

        private void OnHorizontaInput(HorizontalInputEvent e)
        {
            inputDirection.x = e.amount;
        }

        private void OnVerticalInput(VerticalInputEvent e)
        {
            inputDirection.y = e.amount;
        }

        private void activate(CirclePowerInputEvent e)
        {
            if (playerState.activePower == Power.Circle) return;
            if (cooldownTimeLeft > 0f) return;
            playerState.activePower = Power.Circle;

            Vector2 dodgeImpulse = -rb2d.velocity;
            dodgeImpulse += inputDirection.normalized * 10f;
            dodgeImpulse += Vector2.up * 2f;
            Debug.Log("dodgeImpulse = " + (-rb2d.velocity) + " + " + inputDirection.normalized + " * 8f = " + dodgeImpulse);
            rb2d.AddForce(dodgeImpulse, ForceMode2D.Impulse);

            rb2d.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            eventBus.Subscribe<UpdateEvent>(reduceTimeLeft);
            eventBus.Publish(new OnBeginDodge());
        }

        private void reduceTimeLeft(UpdateEvent e)
        {
            powerTimeLeft -= Time.deltaTime;
            if (0 < powerTimeLeft) return;
            deactivate();
        }

        private void deactivate()
        {
            playerState.activePower = Power.None;
            eventBus.Unsubscribe<UpdateEvent>(reduceTimeLeft);

            cooldownTimeLeft = movementValues.circlePowerCooldown;
            eventBus.Subscribe<UpdateEvent>(reduceCooldown);

            rb2d.transform.localScale = Vector3.one;
        }

        private void reduceCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft > 0f) return;

            eventBus.Unsubscribe<UpdateEvent>(reduceCooldown);
        }
    }
}