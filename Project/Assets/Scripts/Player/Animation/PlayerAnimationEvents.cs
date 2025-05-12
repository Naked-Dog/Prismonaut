using UnityEngine;

namespace PlayerSystem
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        private PlayerSoundsEnum[] stepSounds = new[]
        {
            PlayerSoundsEnum.Step1,
            PlayerSoundsEnum.Step2,
            PlayerSoundsEnum.Step3,
            PlayerSoundsEnum.Step4,
        };
        private EventBus eventBus;

        public void SetEventBus(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void PublishRespawnEvent()
        {
            eventBus.Publish(new RequestRespawn());
        }

        public void PlayStepSound()
        {
            var randomSound = stepSounds[Random.Range(0, stepSounds.Length)];
            AudioManager.Instance.Play2DSound(randomSound, 0.5f, false);
        }
        public void PlayJumpSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Jump, 0.5f, false);
        }
        public void PlayDefeatSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Defeat, 0.5f, false);
        }
        public void PlayExplodeSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Explode, 0.5f, false);
        }
        public void PlayLandSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Land, 0.5f, false);
        }
        public void PlayHeavyLandSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.HeavyLand, 0.5f, false);
        }
        public void PlayWindFallSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.LoopWindFall, 0.5f, true);
        }
        public void PlayDodgeSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.DodgeTrans, 0.5f, false);
        }
        public void PlayShieldSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.ShieldTrans, 0.5f, false);
        }
        public void PlayParrySound()
        {
            AudioManager.Instance.Stop(PlayerSoundsEnum.ShieldTrans);
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Parry, 0.5f, false);
        }
        public void PlayDrillSound()
        {
            //AudioManager.Instance.Play2DSound(PlayerSoundsEnum.DrillTrans, 0.5f, false);
        }
    }
}
