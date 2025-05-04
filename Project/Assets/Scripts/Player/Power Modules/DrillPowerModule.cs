using System;
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

        private float damageTimer = 0f;
        FixedJoint2D fixedDrillJoint;

        public DrillPowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PhysicsEventsRelay drillPhysicsRelay,
            PhysicsEventsRelay drillExitPhysicsRelay,
            FixedJoint2D drillJoint)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.drillPhysicsRelay = drillPhysicsRelay;
            this.drillExitPhysicsRelay = drillExitPhysicsRelay;
            
            fixedDrillJoint = drillJoint;
            fixedDrillJoint.enabled = false;
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
            rb2d.AddForce(drillDir * currentSpeed, ForceMode2D.Impulse);

            eventBus.Publish(new RequestMovementPause());
            eventBus.Publish(new RequestGravityOff());
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Subscribe<OnFixedUpdate>(Steer);
            drillPhysicsRelay.OnTriggerEnter2DAction.AddListener(ConfirmDrillCollision);
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
                if(rb2d.linearVelocity.magnitude > powersConstants.drillMaxFirstVelocity)
                {
                    rb2d.linearVelocity = rb2d.linearVelocity.normalized * powersConstants.drillMaxFirstVelocity;
                }
                currentSpeed = Mathf.MoveTowards(currentSpeed, powersConstants.drillMaxFirstVelocity, powersConstants.drillAceleration * Time.fixedDeltaTime);
                rb2d.linearVelocity = drillDir * currentSpeed;
            }
            else
            {
                if (inputDirection.magnitude > 0.1f)
                {
                    float angleDiff = Vector2.SignedAngle(drillDir, inputDirection);

                    float maxSteerSpeed  = powersConstants.drillSecondSteeringAmount;
                    float maxDelta = maxSteerSpeed * Time.fixedDeltaTime;
                    float clampedDelta = Mathf.Clamp(angleDiff, -maxDelta, +maxDelta);
                    drillDir = Quaternion.Euler(0, 0, clampedDelta) * drillDir;
                    drillDir.Normalize();
                }
                float targetAngle = Mathf.Atan2(drillDir.y, drillDir.x) * Mathf.Rad2Deg - 90f;
                float newAngle = Mathf.MoveTowardsAngle(
                    rb2d.rotation,
                    targetAngle,
                    powersConstants.drillSecondSteeringAmount * Time.fixedDeltaTime
                );
                rb2d.MoveRotation(newAngle);
                rb2d.linearVelocity = drillDir * powersConstants.drillMinimalSecondVelocity;
            }
        }

        private void ConfirmDrillCollision(Collider2D other)
        {
            if (other.GetComponent<TestFly>())
            {
                playerState.powerTimeLeft = powersConstants.drillSecondPowerDuration;
                drillPhysicsRelay.OnTriggerEnter2DAction.RemoveListener(ConfirmDrillCollision);
                AttachObjectToDrill(other.gameObject);
                drillPhysicsRelay.OnTriggerExit2DAction.AddListener(ConfirmDrillExit);
                if(other.gameObject.CompareTag("Enemy"))
                {
                    eventBus.Subscribe<OnUpdate>(DamageTimer);
                }
            } 
            else if(other.gameObject.CompareTag("Enemy"))
            {
                enemyCollider = other.GetComponent<Collider2D>();  
                Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
                eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
                isInside = true;
                drillPhysicsRelay.OnTriggerExit2DAction.AddListener(ConfirmDrillExit);
                eventBus.Subscribe<OnUpdate>(DamageTimer);  
            } 
            else if(other.gameObject.CompareTag("HeavyTerrain"))
            {
                heavyTilemapCollider = other.GetComponent<TilemapCollider2D>();
                heavyCompositeCollider= other.GetComponent<CompositeCollider2D>();
                Physics2D.IgnoreCollision(playerCollider, heavyTilemapCollider, true);
                Physics2D.IgnoreCollision(playerCollider, heavyCompositeCollider, true);
                eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
                isInside = true;
                drillPhysicsRelay.OnTriggerExit2DAction.AddListener(ConfirmDrillExit);
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
                else
                {
                    Deactivate();
                    eventBus.Publish(new RequestOppositeReaction());
                }
            }
        }

        private void ConfirmDrillExit(Collider2D other)
        {
            if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("HeavyTerrain"))
            {
                eventBus.Unsubscribe<OnFixedUpdate>(Steer);
                eventBus.Unsubscribe<OnUpdate>(DamageTimer);
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
            var enemyRb = gameObject.GetComponent<Rigidbody2D>();

            fixedDrillJoint.enabled = true;
            fixedDrillJoint.connectedBody = enemyRb;
            fixedDrillJoint.autoConfigureConnectedAnchor = false;

            Vector2 worldTip = drillPhysicsRelay.transform.position;
            Vector2 localTip = rb2d.transform.InverseTransformPoint(worldTip);
            fixedDrillJoint.anchor = localTip;

            Vector2 enemyLocal = enemyRb.transform.InverseTransformPoint(worldTip);
            fixedDrillJoint.connectedAnchor = enemyLocal;

            isSecondStage = true;
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
            if (timeToReturnSteer > 0) return;
            eventBus.Subscribe<OnHorizontalInput>(TakeHorizontalInputDirection);
            eventBus.Subscribe<OnVerticalInput>(TakeVerticalInputDirection);
            eventBus.Unsubscribe<OnUpdate>(SteerControlReturn);
            timeToReturnSteer = 0.4f;
        }

        private void DamageTimer(OnUpdate e)
        {
            if(damageTimer <= 0)
            {
                Debug.Log("Damage");
                //Here, call the method that deals damage to the enemy. The damage value is stored in the scriptable constants. 
                damageTimer += 1f;
            }

            damageTimer -= Time.deltaTime;
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            fixedDrillJoint.enabled = false;
            rb2d.MoveRotation(0f);
            drillPhysicsRelay.transform.rotation = Quaternion.Euler(0, 0, 0);
            currentSpeed = 0f;
            damageTimer = 0f;
            isInside = false;
            eventBus.Publish(new RequestMovementResume());
            eventBus.Publish(new RequestGravityOn());
            eventBus.Unsubscribe<OnFixedUpdate>(Steer);
            drillPhysicsRelay.OnTriggerEnter2DAction.RemoveListener(ConfirmDrillCollision);
            drillPhysicsRelay.OnTriggerExit2DAction.RemoveListener(ConfirmDrillExit);
            drillExitPhysicsRelay.OnTriggerExit2DAction.RemoveListener(ConfirmPlayerExit);
        }
    }
}