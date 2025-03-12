using UnityEngine;

namespace PlayerSystem
{
    public class DrillPowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PlayerMovementScriptable movementValues;

        private Vector2 inputDirection = Vector2.zero;

        public DrillPowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PlayerMovementScriptable movementValues)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.movementValues = movementValues;

            Debug.Log("Drill Power Module created");
            eventBus.Subscribe<OnTrianglePowerInput>(OnTrianglePowerInput);
        }

        private void OnTrianglePowerInput(OnTrianglePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            Activate();
        }

        private void Activate()
        {
            playerState.activePower = Power.Drill;
            playerState.powerTimeLeft = 2f;
            rb2d.gravityScale = 0;

            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Publish(new RequestMovementPause());
            eventBus.Subscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Subscribe<OnVerticalInput>(OnVerticalInput);
            eventBus.Subscribe<OnFixedUpdate>(OnFixedUpdate);
        }

        private void OnHorizontalInput(OnHorizontalInput e)
        {
            inputDirection.x = e.amount;
        }

        private void OnVerticalInput(OnVerticalInput e)
        {
            inputDirection.y = e.amount;
        }

        private void OnFixedUpdate(OnFixedUpdate e)
        {
            float angle = Vector2.SignedAngle(rb2d.velocity, inputDirection);
            float rotationAmount = Mathf.Sign(angle) * 3f;
            rb2d.velocity = Quaternion.Euler(0, 0, rotationAmount) * rb2d.velocity;
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (0 < playerState.powerTimeLeft) return;
            Deactivate();
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            rb2d.gravityScale = 3f;

            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Publish(new RequestMovementResume());
            eventBus.Unsubscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Unsubscribe<OnVerticalInput>(OnVerticalInput);
            eventBus.Unsubscribe<OnFixedUpdate>(OnFixedUpdate);
        }
    }
}