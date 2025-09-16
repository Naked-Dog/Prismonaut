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
            AudioManager.Instance.Play2DSound(randomSound, false);
        }
        public void PlayJumpSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Jump, false);
        }
        public void PlayDefeatSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Defeat, false);
        }
        public void PlayExplodeSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Explode, false);
        }
        public void PlayLandSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Land, false);
        }
        public void PlayHeavyLandSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.HeavyLand, false);
        }
        public void PlayWindFallSound()
        {
            //AudioManager.Instance.Play2DSound(PlayerSoundsEnum.LoopWindFall, 1f, true);
        }
        public void PlayDodgeSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.DodgeTrans, false);
        }
        public void PlayShieldSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.ShieldTrans, false);
        }
        public void PlayParrySound()
        {
            AudioManager.Instance?.Stop(PlayerSoundsEnum.ShieldTrans);
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Parry, false);
        }
        public void PlayHurtSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.Hurt, false);
        }

        public void PlayRegularFormSound()
        {
            AudioManager.Instance.Play2DSound(PlayerSoundsEnum.RegularForm, false);
        }
    }
}
