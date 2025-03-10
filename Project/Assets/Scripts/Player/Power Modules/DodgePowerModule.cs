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
        private bool isActive = false;

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
            if (isActive) return;
            if (cooldownTimeLeft > 0f) return;
            isActive = true;
            playerState.activePower = Power.Circle;

            Vector2 dodgeImpulse = -rb2d.velocity;
            dodgeImpulse += inputDirection.normalized * 10f;
            dodgeImpulse += Vector2.up * 2f;
            Debug.Log("dodgeImpulse = " + (-rb2d.velocity) + " + " + inputDirection.normalized + " * 8f = " + dodgeImpulse);
            rb2d.AddForce(dodgeImpulse, ForceMode2D.Impulse);

            // directionValue = playerState.facingDirection == Direction.Right ? 1 : -1;
            // rb2d.velocity = new Vector2(movementValues.circlePowerForce * directionValue, 0f);
            // powerTimeLeft = movementValues.circlePowerDuration;

            eventBus.Subscribe<UpdateEvent>(reduceTimeLeft);
            // eventBus.Publish(new ToggleCirclePowerEvent(true));
        }

        private void reduceTimeLeft(UpdateEvent e)
        {
            powerTimeLeft -= Time.deltaTime;
            if (0 < powerTimeLeft) return;
            deactivate();
        }

        private void deactivate()
        {
            isActive = false;
            playerState.activePower = Power.None;
            eventBus.Unsubscribe<UpdateEvent>(reduceTimeLeft);

            cooldownTimeLeft = movementValues.circlePowerCooldown;
            eventBus.Subscribe<UpdateEvent>(reduceCooldown);

            // eventBus.Publish(new ToggleCirclePowerEvent(false));
        }

        private void reduceCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft > 0f) return;

            eventBus.Unsubscribe<UpdateEvent>(reduceCooldown);
        }
    }
}