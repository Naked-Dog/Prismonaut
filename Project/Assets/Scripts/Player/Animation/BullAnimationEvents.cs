using UnityEngine;

public class BullAnimationEvents : MonoBehaviour
{
    private BullSoundsEnum[] stepSounds = new[]
    {
        BullSoundsEnum.Step1,
        BullSoundsEnum.Step2,
        BullSoundsEnum.Step3,
    };

    public void PlayStepSound()
    {
        var randomSound = stepSounds[Random.Range(0, stepSounds.Length)];
        AudioManager.Instance.Play2DSound(randomSound, 0.5f, false);
    }

    public void PlayRushChargeSound()
    {
        AudioManager.Instance.Play2DSound(BullSoundsEnum.RushCharge, 0.5f, false);
    }

    public void PlayRushLoopSound()
    {
        AudioManager.Instance.Play2DSound(BullSoundsEnum.LoopRush, 0.5f, false);
    }

    public void StopRushLoopSound()
    {
        AudioManager.Instance?.Stop(BullSoundsEnum.LoopRush);
    }

    public void PlayDetectedSound()
    {
        AudioManager.Instance.Play2DSound(BullSoundsEnum.Detected, 0.5f, false);
    }

    public void PlayHurtSound()
    {
        AudioManager.Instance.Play2DSound(BullSoundsEnum.Hurt, 0.5f, false);
    }

    public void PlayDeathSound()
    {
        AudioManager.Instance.Play2DSound(BullSoundsEnum.Death, 0.5f, false);
    }
}
