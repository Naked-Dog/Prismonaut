using UnityEngine;

namespace PlayerSystem
{
    public class CancelPowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private PlayerPowersScriptable powersConstants;
           
        public CancelPowerModule(EventBus eventBus, PlayerState playerState)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            powersConstants = GlobalConstants.Get<PlayerPowersScriptable>();
            eventBus.Subscribe<OnCancelPower>(Activate);
        }

        private void Activate(OnCancelPower e)
        {
            playerState.activePower = Power.Cancel;
            playerState.powerTimeLeft = powersConstants.cancelPowerTime;
            eventBus.Subscribe<OnFixedUpdate>(ReduceCancelTime);
        }

        private void ReduceCancelTime(OnFixedUpdate e)
        {
            playerState.powerTimeLeft -= Time.fixedDeltaTime;
            if(playerState.powerTimeLeft > 0f) return;
            Deactivate();
        }

        private void Deactivate()
        {
            playerState.activePower = Power.None;
            eventBus.Unsubscribe<OnFixedUpdate>(ReduceCancelTime);
        }

    }
}
