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
            eventBus.Subscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
            eventBus.Subscribe<OnVerticalInput>(TakeVerticalInputDirection);
            eventBus.Subscribe<OnFixedUpdate>(Steer);
        }

        private void TakeHorizontalInputDirection(OnHorizontalInput e)
        {
            inputDirection.x = e.amount;
        }

        private void TakeVerticalInputDirection(OnVerticalInput e)
        {
            inputDirection.y = e.amount;
        }

        private void Steer(OnFixedUpdate e)
        {
            if (0.1f < inputDirection.magnitude)
            {
                float angleChange = Vector2.SignedAngle(powerVelocity, inputDirection);
                float steerAmount = isSecondStage ? movementValues.drillSecondSteeringAmount : movementValues.drillFirstSteeringAmount;
                float rotationAmount = Mathf.Sign(angleChange) * steerAmount;
                powerVelocity = Quaternion.Euler(0, 0, rotationAmount) * powerVelocity;

            }
            float angle = Mathf.Atan2(playerState.velocity.y, playerState.velocity.x) * Mathf.Rad2Deg - 90f;
            Debug.Log(angle);
            rb2d.linearVelocity = powerVelocity;
            rb2d.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (0 < playerState.powerTimeLeft) return;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            Deactivate();
            // BeginSecondStage();
        }

        private void BeginSecondStage()
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
            rb2d.transform.rotation = Quaternion.identity;

            eventBus.Publish(new RequestMovementResume());
            eventBus.Publish(new RequestGravityOn());
            eventBus.Unsubscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
            eventBus.Unsubscribe<OnVerticalInput>(TakeVerticalInputDirection);
            eventBus.Unsubscribe<OnFixedUpdate>(Steer);
        }
    }
}