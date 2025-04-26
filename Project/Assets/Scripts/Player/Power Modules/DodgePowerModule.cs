using UnityEngine;

namespace PlayerSystem
{
    public class DodgePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PlayerPowersScriptable movementValues;

        private Vector2 inputDirection = Vector2.zero;
        private Vector2 appliedDirection = Vector2.zero;

        public DodgePowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.movementValues = GlobalConstants.Get<PlayerPowersScriptable>();

            eventBus.Subscribe<OnCirclePowerInput>(OnCirclePowerInput);
            eventBus.Subscribe<OnHorizontalInput>(OnHorizontaInput);
            eventBus.Subscribe<OnVerticalInput>(OnVerticalInput);
        }

        private void OnHorizontaInput(OnHorizontalInput e)
        {
            inputDirection.x = e.amount;
        }

        private void OnVerticalInput(OnVerticalInput e)
        {
            inputDirection.y = e.amount;
        }

        private void OnCirclePowerInput(OnCirclePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            if (inputDirection.magnitude < 0.1f) return;

            Activate();
        }

        private void Activate()
        {
            Vector2 dodgeImpulse = -rb2d.linearVelocity;
            appliedDirection = inputDirection;
            Vector2 normalInputDirection = inputDirection.normalized;
            dodgeImpulse += normalInputDirection * movementValues.dodgePowerForce;
            dodgeImpulse += Vector2.up * Mathf.Abs(normalInputDirection.x) * 5f;
            rb2d.AddForce(dodgeImpulse, ForceMode2D.Impulse);

            eventBus.Publish(new OnDodgeActivation());
            playerState.activePower = Power.Dodge;

            playerState.powerTimeLeft = movementValues.dodgePowerDuration;
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            rb2d.AddForce(-appliedDirection * movementValues.dodgePowerBreakForce);
            if (0 < playerState.powerTimeLeft) return;
            Deactivate();
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);

            rb2d.transform.localScale = Vector3.one;
        }
    }
}