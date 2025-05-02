using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PlayerSystem
{
    public class DrillPowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PhysicsEventsRelay drillPhysicsRelay;
        private PhysicsEventsRelay drillExitPhysicsRelay;
        private HingeJoint2D drillJoint;
        private PlayerPowersScriptable powersConstants;

        private Vector2 inputDirection = Vector2.zero;
        private bool isSecondStage = false;
        private Vector2 drillDir = Vector2.right;
        private bool isFacingRight => playerState.facingDirection == Direction.Right;

        private Collider2D playerCollider;
        private Collider2D enemyCollider;
        private Collider2D heavyTilemapCollider;
        private Collider2D heavyCompositeCollider;
        private float rotationVelocity;   
        private bool isInside;

        public DrillPowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PhysicsEventsRelay drillPhysicsRelay,
            PhysicsEventsRelay drillExitPhysicsRelay,
            HingeJoint2D drillJoint)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.drillPhysicsRelay = drillPhysicsRelay;
            this.drillExitPhysicsRelay = drillExitPhysicsRelay;
            this.drillJoint = drillJoint;
            powersConstants = GlobalConstants.Get<PlayerPowersScriptable>();
            playerCollider = rb2d.gameObject.transform.GetChild(1).GetComponent<Collider2D>();

            eventBus.Subscribe<OnTrianglePowerInput>(OnTrianglePowerInput);
            eventBus.Subscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
            eventBus.Subscribe<OnVerticalInput>(TakeVerticalInputDirection);
        }

        private void OnTrianglePowerInput(OnTrianglePowerInput e)
        {
            if (playerState.activePower == Power.Drill && isSecondStage)
            {
                Deactivate();
                return;
            }

            if (playerState.activePower != Power.None) return;

            Activate();
        }

        private void Activate()
        {
            playerState.activePower = Power.Drill;
            playerState.powerTimeLeft = powersConstants.drillFirstPowerDuration;
            isSecondStage = false;

            if (inputDirection.sqrMagnitude > 0.1f)
            {
                drillDir = inputDirection;
            }
            else
            {
                drillDir = isFacingRight ? Vector2.right : Vector2.left;
            }

            float initSpeed = powersConstants.drillMinimalFirstVelocity;
            rb2d.linearVelocity = drillDir * initSpeed;

            eventBus.Publish(new RequestMovementPause());
            eventBus.Publish(new RequestGravityOff());
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Subscribe<OnFixedUpdate>(Steer);
            drillPhysicsRelay.OnTriggerEnter2DAction.AddListener(ConfirmDrillCollision);
            drillPhysicsRelay.OnTriggerExit2DAction.AddListener(ConfirmDrillExit);
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
            if (!isSecondStage)
            {
                float targetAngle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;
                drillPhysicsRelay.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
                if (inputDirection.sqrMagnitude < 0.1f)
                {
                    rb2d.linearVelocity = rb2d.transform.up * powersConstants.drillMinimalFirstVelocity;
                    return;
                }
                float smoothTime = 0.1f;
                float newAngle = Mathf.SmoothDampAngle(
                    rb2d.rotation,    
                    targetAngle,      
                    ref rotationVelocity,
                    smoothTime
                );
                rb2d.MoveRotation(newAngle);
                rb2d.linearVelocity = rb2d.transform.up * powersConstants.drillMinimalFirstVelocity;

                // if (inputDirection.sqrMagnitude < 0.1f) return;

                // float targetAngle = Mathf.Atan2(drillDir.y, drillDir.x) * Mathf.Rad2Deg - 90f;
                // float newAngle = Mathf.Lerp(rb2d.rotation, targetAngle, powersConstants.drillFirstSteeringAmount * Time.fixedDeltaTime);

                // float velocity = powersConstants.drillMinimalFirstVelocity;
                // float rotationAmount = Mathf.Sign(newAngle);
                // drillDir = Quaternion.Euler(0, 0, rotationAmount) * drillDir;
                // rb2d.MoveRotation(newAngle);
                // rb2d.linearVelocity = inputDirection * velocity;

                // float angleDiff = Vector2.SignedAngle(drillDir, inputDirection);
                // if(Mathf.Approximately(angleDiff,0f)) return;
                // float steerSpeed = isSecondStage ? powersConstants.drillSecondSteeringAmount : powersConstants.drillFirstSteeringAmount;
                // if (Mathf.Abs(angleDiff) < steerSpeed)
                // {
                //     drillDir = inputDirection.normalized;
                // }
                // else
                // {
                //     float rotationAmount = Mathf.Sign(angleDiff) * steerSpeed;
                //     drillDir = Quaternion.Euler(0, 0, rotationAmount) * drillDir;
                // }
                // float currentSpeed = rb2d.linearVelocity.magnitude;
                // float maxSpeed = powersConstants.drillMaxFirstVelocity;
                // float speed = Mathf.Min(currentSpeed, maxSpeed);
                // rb2d.linearVelocity = drillDir.normalized * speed;

            }
            else
            {
                if (inputDirection.magnitude < 0.1f) return;
                float angleChange = Vector2.SignedAngle(playerState.velocity, inputDirection);
                JointMotor2D motor = drillJoint.motor;
                if(Mathf.Approximately(angleChange,0f))
                {
                    motor.motorSpeed = 0f;
                } 
                else
                {
                    float steerAmount = isSecondStage ? powersConstants.drillSecondSteeringAmount : powersConstants.drillFirstSteeringAmount;
                    float rotationAmount = Mathf.Sign(angleChange) * steerAmount;
                    motor.motorSpeed = rotationAmount * powersConstants.drillSecondSteeringAmount;
                }
                drillJoint.motor = motor;
            }

        }

        private void ConfirmDrillCollision(Collider2D other)
        {
            if (other.GetComponent<TestFly>())
            {
                eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
                drillPhysicsRelay.OnTriggerEnter2DAction.RemoveListener(ConfirmDrillCollision);
                AttachObjectToDrill(other.gameObject);
                eventBus.Subscribe<OnFixedUpdate>(DrillIntoGameObject);
            } 
            else if(other.gameObject.CompareTag("Enemy"))
            {
                enemyCollider = other.GetComponent<Collider2D>();  
                Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
                eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);  
                isInside = true;
            } 
            else if(other.gameObject.CompareTag("HeavyTerrain"))
            {
                heavyTilemapCollider = other.GetComponent<TilemapCollider2D>();
                heavyCompositeCollider= other.GetComponent<CompositeCollider2D>();
                Physics2D.IgnoreCollision(playerCollider, heavyTilemapCollider, true);
                Physics2D.IgnoreCollision(playerCollider, heavyCompositeCollider, true);
                eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            }
        }

        private void ConfirmDrillExit(Collider2D other)
        {
            if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("HeavyTerrain"))
            {
                eventBus.Unsubscribe<OnFixedUpdate>(Steer);
                drillExitPhysicsRelay.OnTriggerExit2DAction.AddListener(ConfirmPlayerExit);
            }
        }

        private void ConfirmPlayerExit(Collider2D other)
        {
            if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("HeavyTerrain"))
            {
                if(enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, enemyCollider, false);
                } 
            
                if(heavyCompositeCollider != null && heavyTilemapCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, heavyTilemapCollider, false);
                Physics2D.IgnoreCollision(playerCollider, heavyCompositeCollider, false);
                }
                    
                Deactivate();
                rb2d.linearVelocity *= 3f;
            }
        }

        private void AttachObjectToDrill(GameObject gameObject)
        {
            drillJoint.enabled = true;
            drillJoint.connectedBody = gameObject.GetComponent<Rigidbody2D>();
            drillJoint.connectedAnchor = gameObject.transform.position - rb2d.transform.position;
            playerState.powerTimeLeft = powersConstants.drillSecondPowerDuration;
            isSecondStage = true;
            if (rb2d.linearVelocity.magnitude < powersConstants.drillMinimalSecondVelocity)
            {
                rb2d.linearVelocity = rb2d.linearVelocity.normalized * powersConstants.drillMinimalSecondVelocity;
            }
            eventBus.Publish(new RequestGravityOff());
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
        }

        private void DrillIntoGameObject(OnFixedUpdate e)
        {
            Vector2 drillingVector = drillJoint.connectedBody.transform.position - rb2d.transform.position;
            rb2d.linearVelocity = drillingVector.normalized * rb2d.linearVelocity.magnitude;
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (playerState.powerTimeLeft > 0) return;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            Deactivate();
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            drillJoint.enabled = false;
            rb2d.MoveRotation(0f);
            eventBus.Publish(new RequestMovementResume());
            eventBus.Publish(new RequestGravityOn());
            eventBus.Unsubscribe<OnFixedUpdate>(Steer);
            eventBus.Unsubscribe<OnFixedUpdate>(DrillIntoGameObject);
            drillPhysicsRelay.OnTriggerEnter2DAction.RemoveListener(ConfirmDrillCollision);
            drillPhysicsRelay.OnTriggerExit2DAction.RemoveListener(ConfirmDrillExit);
            drillExitPhysicsRelay.OnTriggerExit2DAction.RemoveListener(ConfirmPlayerExit);
        }
    }
}