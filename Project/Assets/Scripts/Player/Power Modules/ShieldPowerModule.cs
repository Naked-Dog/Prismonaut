using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerSystem
{
    public class ShieldPowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private PlayerPowersScriptable powersConstants;
        private UnityEvent activatePower;
        private PlayerBaseModule baseModule;

        private Vector2 savedVelocity;

        public ShieldPowerModule(
            EventBus eventBus,
            PlayerState playerState,
            Rigidbody2D rb2d,
            PlayerBaseModule baseModule)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.powersConstants = GlobalConstants.Get<PlayerPowersScriptable>();
            this.baseModule = baseModule;
            //this.activatePower.AddListener(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().G);

            eventBus.Subscribe<OnSquarePowerInput>(OnSquarePowerInput);
        }

        private void OnSquarePowerInput(OnSquarePowerInput e)
        {
            if (playerState.activePower != Power.None) return;
            Activate();
        }

        private void Activate()
        {
            if (playerState.currentCharges <= 0) return;
            playerState.currentCharges--;
            baseModule.StartChargeRegeneration();
            eventBus.Publish(new RequestMovementPause());
            playerState.activePower = Power.Shield;
            playerState.powerTimeLeft = powersConstants.shieldPowerDuration;
            savedVelocity = rb2d.linearVelocity;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.gravityScale = 0;
            eventBus.Subscribe<OnUpdate>(ReduceTimeLeft);
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
            rb2d.linearVelocity = savedVelocity;
            eventBus.Unsubscribe<OnUpdate>(ReduceTimeLeft);
            eventBus.Publish(new RequestMovementResume());
        }
    }
}