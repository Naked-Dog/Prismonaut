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
        private Vector2 powerVelocity = Vector2.zero;
        private bool isSecondStage = false;

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

            eventBus.Subscribe<OnTrianglePowerInput>(OnTrianglePowerInput);
        }

        private void OnTrianglePowerInput(OnTrianglePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            if (playerState.velocity.magnitude < 0.1f) return;
            Activate();
        }

        private void Activate()
        {
            playerState.activePower = Power.Drill;
            playerState.powerTimeLeft = movementValues.drillFirstPowerDuration;
            isSecondStage = false;
            if (playerState.velocity.magnitude < movementValues.drillMinimalFirstVelocity)
            {
                rb2d.linearVelocity = playerState.velocity.normalized * movementValues.drillMinimalFirstVelocity;
            }
            powerVelocity = rb2d.linearVelocity;

            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Publish(new RequestMovementPause());
            eventBus.Publish(new RequestGravityOff());
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
            if (0.1f < inputDirection.magnitude)
            {
                // Steer
                float angle = Vector2.SignedAngle(powerVelocity, inputDirection);
                float steerAmount = isSecondStage ? movementValues.drillSecondSteeringAmount : movementValues.drillFirstSteeringAmount;
                float rotationAmount = Mathf.Sign(angle) * steerAmount;
                powerVelocity = Quaternion.Euler(0, 0, rotationAmount) * powerVelocity;

            }
            rb2d.linearVelocity = powerVelocity;
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (0 < playerState.powerTimeLeft) return;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            StartDrillingForReal();
        }

        private void StartDrillingForReal()
        {
            playerState.powerTimeLeft = movementValues.drillSecondPowerDuration;
            isSecondStage = true;
            if (rb2d.linearVelocity.magnitude < movementValues.drillMinimalSecondVelocity)
            {
                powerVelocity = rb2d.linearVelocity.normalized * movementValues.drillMinimalSecondVelocity;
                rb2d.linearVelocity = powerVelocity;
            }
            eventBus.Subscribe<OnUpdate>(MoreReduceTimeLeft);
        }

        private void MoreReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (0 < playerState.powerTimeLeft) return;
            eventBus.Unsubscribe<OnUpdate>(MoreReduceTimeLeft);
            Deactivate();
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;

            eventBus.Publish(new RequestMovementResume());
            eventBus.Publish(new RequestGravityOn());
            eventBus.Unsubscribe<OnHorizontalInput>(OnHorizontalInput);
            eventBus.Unsubscribe<OnVerticalInput>(OnVerticalInput);
            eventBus.Unsubscribe<OnFixedUpdate>(OnFixedUpdate);
        }
    }
}