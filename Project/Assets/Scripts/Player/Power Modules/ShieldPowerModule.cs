using UnityEngine;

namespace PlayerSystem
{
    public class ShieldPowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PlayerPowersScriptable powersConstants;
        private PlayerBaseModule baseModule;

        private Vector2 savedVelocity;
        private PhysicsEventsRelay shieldPhysicsRelay;
        Collider2D shieldCollider;
        private float parryTime;

        public ShieldPowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PhysicsEventsRelay shieldPhysicsRelay, PlayerBaseModule baseModule)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.shieldPhysicsRelay = shieldPhysicsRelay;
            shieldCollider = shieldPhysicsRelay.GetComponent<Collider2D>();
            shieldCollider.enabled = false;
            this.baseModule = baseModule;

            powersConstants = GlobalConstants.Get<PlayerPowersScriptable>();

            eventBus.Subscribe<OnSquarePowerInput>(OnSquarePowerInput);
        }

        private void OnSquarePowerInput(OnSquarePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            Activate();
        }

        private void Activate()
        {
            if (playerState.currentCharges < 1f) return;
            baseModule.StartChargeRegeneration();
            eventBus.Publish(new RequestMovementPause());
            playerState.activePower = Power.Shield;
            playerState.powerTimeLeft = powersConstants.shieldPowerDuration;
            parryTime = playerState.powerTimeLeft - powersConstants.parryDuration;
            savedVelocity = rb2d.linearVelocity;
            rb2d.linearVelocity = Vector2.zero;
            eventBus.Publish(new RequestGravityOff());
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
            shieldCollider.enabled = true;
            shieldPhysicsRelay.OnTriggerEnter2DAction.AddListener(ShieldCollision);
            playerState.isParry = true;
        }

        private void ReduceTimeLeft(OnUpdate e)
        {
            playerState.powerTimeLeft -= Time.deltaTime;
            if (playerState.powerTimeLeft < parryTime) playerState.isParry = false;
            if (0 < playerState.powerTimeLeft) return;
            Deactivate();
        }

        private void ShieldCollision(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (playerState.isParry)
                {
                    playerState.activePower = Power.Parry;
                    ReflectDamage();
                }
                else
                {
                    //get enemy damage
                    //rb2d.gameObject.GetComponent<PlayerBaseModule>().healthModule.Damage(damageAmount)
                    Deactivate();
                }
            }

            if(other.CompareTag("Slime"))
            {
                if(playerState.isParry)
                {
                    playerState.activePower = Power.Parry;
                }
            }
        }

        private void ReflectDamage()
        {
            float damage = powersConstants.reflectionDamage;
            //Emit enemy get damage logic.
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            rb2d.linearVelocity = savedVelocity;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Publish(new RequestMovementResume());
            eventBus.Publish(new RequestGravityOn());
            shieldPhysicsRelay.OnTriggerEnter2DAction.RemoveListener(ShieldCollision);
            shieldCollider.enabled = false;
        }
    }
}