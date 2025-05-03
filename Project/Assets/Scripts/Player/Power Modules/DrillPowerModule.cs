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
        private float currentSpeed = 0f;

        private float timeToReturnSteer = 0.4f;
        private bool isInside = false;

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
                drillDir = isFacingRight ? rb2d.transform.right : -rb2d.transform.right;
            }
            currentSpeed = powersConstants.drillMinimalFirstVelocity;
            rb2d.linearVelocity = drillDir * powersConstants.drillMinimalFirstVelocity;

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
                if (inputDirection.sqrMagnitude > 0.1f)
                {
                    drillDir = inputDirection.normalized;
                }
                float targetAngle = Mathf.Atan2(drillDir.y, drillDir.x) * Mathf.Rad2Deg - 90f;
                float smoothTime = 0.13f;
                float newAngle = Mathf.SmoothDampAngle(
                    rb2d.rotation,    
                    targetAngle,      
                    ref rotationVelocity,
                    smoothTime
                );
                rb2d.MoveRotation(newAngle);
                drillPhysicsRelay.transform.rotation = Quaternion.Euler(0, 0, newAngle);
                currentSpeed = Mathf.MoveTowards(currentSpeed, powersConstants.drillMaxFirstVelocity, powersConstants.drillAceleration * Time.fixedDeltaTime);
                rb2d.linearVelocity = drillDir * currentSpeed;
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
                isInside = true;
            }
            else if(other.gameObject.CompareTag("Ground"))
            {
                if(isInside)
                {
                    eventBus.Unsubscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
                    eventBus.Unsubscribe<OnVerticalInput>(TakeVerticalInputDirection);
                    eventBus.Subscribe<OnUpdate>(SteerControlReturn);
                    inputDirection = Vector2.zero;
                    drillDir = -drillDir;
                    return;
                }
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

        private void SteerControlReturn(OnUpdate e)
        {
            timeToReturnSteer -= Time.deltaTime;
            Debug.Log(timeToReturnSteer);
            if (timeToReturnSteer > 0) return;
            eventBus.Subscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
            eventBus.Subscribe<OnVerticalInput>(TakeVerticalInputDirection);
            eventBus.Unsubscribe<OnUpdate>(SteerControlReturn);
            timeToReturnSteer = 0.4f;
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            drillJoint.enabled = false;
            rb2d.MoveRotation(0f);
            drillPhysicsRelay.transform.rotation = Quaternion.Euler(0, 0, 0);
            currentSpeed = 0f;
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