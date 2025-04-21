using UnityEngine;

namespace PlayerSystem
{
    public class DrillPowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private TriggerEventHandler drillTrigger;
        private HingeJoint2D drillJoint;
        private PlayerPowersScriptable powersConstants;

        private Vector2 inputDirection = Vector2.zero;
        private bool isSecondStage = false;

        public DrillPowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            TriggerEventHandler drillTrigger,
            HingeJoint2D drillJoint,
            PlayerPowersScriptable powersConstants)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.drillTrigger = drillTrigger;
            this.drillJoint = drillJoint;
            this.powersConstants = powersConstants;

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
            playerState.powerTimeLeft = powersConstants.drillFirstPowerDuration;
            isSecondStage = false;
            if (playerState.velocity.magnitude < powersConstants.drillMinimalFirstVelocity)
            {
                rb2d.linearVelocity = playerState.velocity.normalized * powersConstants.drillMinimalFirstVelocity;
            }

            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Publish(new RequestMovementPause());
            eventBus.Publish(new RequestGravityOff());
            eventBus.Subscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
            eventBus.Subscribe<OnVerticalInput>(TakeVerticalInputDirection);
            eventBus.Subscribe<OnFixedUpdate>(Steer);
            drillTrigger.OnTriggerEnter2DAction.AddListener(ConfirmDrillCollision);
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
            // float angle = Mathf.Atan2(playerState.velocity.y, playerState.velocity.x) * Mathf.Rad2Deg - 90f;
            // drillTrigger.transform.rotation = Quaternion.Euler(0, 0, angle);

            float angleChange = Vector2.SignedAngle(playerState.velocity, inputDirection);
            float steerAmount = isSecondStage ? powersConstants.drillSecondSteeringAmount : powersConstants.drillFirstSteeringAmount;
            float rotationAmount = Mathf.Sign(angleChange) * steerAmount;
            drillJoint.GetComponent<Rigidbody2D>().AddTorque(50);
            rb2d.linearVelocity = Quaternion.Euler(0, 0, rotationAmount) * playerState.velocity;


        }

        private void ConfirmDrillCollision(Collider2D other)
        {
            if (!other.GetComponent<TestFly>()) return;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            drillTrigger.OnTriggerEnter2DAction.RemoveListener(ConfirmDrillCollision);
            DrillIntoGameObject(other.gameObject);
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (0 < playerState.powerTimeLeft) return;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            Deactivate();
        }

        private void DrillIntoGameObject(GameObject gameObject)
        {
            drillJoint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
            drillJoint.connectedAnchor = gameObject.transform.position - rb2d.transform.position;
            playerState.powerTimeLeft = powersConstants.drillSecondPowerDuration;
            isSecondStage = true;
            if (rb2d.linearVelocity.magnitude < powersConstants.drillMinimalSecondVelocity)
            {
                rb2d.linearVelocity = rb2d.linearVelocity.normalized * powersConstants.drillMinimalSecondVelocity;
            }
            eventBus.Publish(new RequestGravityOff());
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
            drillTrigger.OnTriggerEnter2DAction.RemoveListener(ConfirmDrillCollision);
        }
    }
}