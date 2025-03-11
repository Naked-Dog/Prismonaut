using UnityEngine;

namespace PlayerSystem
{
    public class DodgePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PlayerMovementScriptable movementValues;

        private Vector2 inputDirection = Vector2.zero;
        private float powerTimeLeft = 0f;

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

            eventBus.Subscribe<OnHorizontalInput>(OnHorizontaInput);
            eventBus.Subscribe<OnVerticalInput>(OnVerticalInput);
            eventBus.Subscribe<OnCirclePowerInput>(activate);
        }

        private void OnHorizontaInput(OnHorizontalInput e)
        {
            inputDirection.x = e.amount;
        }

        private void OnVerticalInput(OnVerticalInput e)
        {
            inputDirection.y = e.amount;
        }

        private void activate(OnCirclePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            if (inputDirection.magnitude < 0.1f) return;

            Vector2 dodgeImpulse = -rb2d.velocity;
            dodgeImpulse += inputDirection.normalized * movementValues.dodgePowerForce;
            dodgeImpulse += Vector2.up * 2f;
            rb2d.AddForce(dodgeImpulse, ForceMode2D.Impulse);

            eventBus.Publish(new OnDodge());
            playerState.activePower = Power.Dodge;

            powerTimeLeft = movementValues.dodgePowerDuration;
            eventBus.Subscribe<OnUpdate>(reduceTimeLeft);
        }

        private void reduceTimeLeft(OnUpdate e)
        {
            powerTimeLeft -= Time.deltaTime;
            if (0 < powerTimeLeft) return;
            deactivate();
        }

        private void deactivate()
        {
            playerState.activePower = Power.None;
            eventBus.Unsubscribe<OnUpdate>(reduceTimeLeft);

            rb2d.transform.localScale = Vector3.one;
        }
    }
}