using UnityEngine;

namespace PlayerSystem
{
    public class PlayerVFX
    {
        private readonly EventBus eventBus;
        private readonly PlayerState playerState;
        private readonly ParticleSystem dustTrail;
        private readonly ParticleSystem jumpPuff;
        private readonly ParticleSystem landPuff;

        private const float MoveThreshold = 0.1f;
        private const float JumpRiseThreshold = 0.5f;
        private const int JumpBurstCount = 8;
        private const int LandBurstMin = 6;
        private const int LandBurstMax = 20;
        private const float LandBurstPerUnitVelocity = 1.4f;

        private bool dustEmitting;
        private bool jumpEmitting;

        public PlayerVFX(
            EventBus eventBus,
            PlayerState playerState,
            ParticleSystem dustTrail,
            ParticleSystem jumpPuff,
            ParticleSystem landPuff)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.dustTrail = dustTrail;
            this.jumpPuff = jumpPuff;
            this.landPuff = landPuff;

            if (dustTrail != null) dustTrail.Stop();
            if (jumpPuff != null) jumpPuff.Stop();

            eventBus.Subscribe<OnUpdate>(OnUpdate);
            eventBus.Subscribe<OnJumpMovement>(OnJump);
            eventBus.Subscribe<OnLandedMovement>(OnLanded);
        }

        private void OnUpdate(OnUpdate e)
        {
            bool alive = playerState.healthState != HealthState.Death;
            UpdateDustTrail(alive);
            UpdateJumpEmission(alive);
        }

        private void UpdateDustTrail(bool alive)
        {
            if (dustTrail == null) return;

            bool grounded = playerState.groundState == GroundState.Grounded;
            bool moving = Mathf.Abs(playerState.velocity.x) > MoveThreshold;
            bool shouldEmit = grounded && moving && alive;

            if (shouldEmit && !dustEmitting)
            {
                dustTrail.Play();
                dustEmitting = true;
            }
            else if (!shouldEmit && dustEmitting)
            {
                dustTrail.Stop();
                dustEmitting = false;
            }
        }

        private void UpdateJumpEmission(bool alive)
        {
            if (jumpPuff == null) return;

            bool airborne = playerState.groundState == GroundState.Airborne;
            bool rising = playerState.velocity.y > JumpRiseThreshold;
            bool noPower = playerState.activePower == Power.None;
            bool shouldEmit = airborne && rising && noPower && alive;

            if (shouldEmit && !jumpEmitting)
            {
                jumpPuff.Play();
                jumpEmitting = true;
            }
            else if (!shouldEmit && jumpEmitting)
            {
                jumpPuff.Stop();
                jumpEmitting = false;
            }
        }

        private void OnJump(OnJumpMovement e)
        {
            if (jumpPuff == null) return;
            jumpPuff.Emit(JumpBurstCount);
        }

        private void OnLanded(OnLandedMovement e)
        {
            if (landPuff == null) return;

            int burstCount = Mathf.Clamp(
                Mathf.RoundToInt(LandBurstMin + e.fallVelocity * LandBurstPerUnitVelocity),
                LandBurstMin,
                LandBurstMax);

            landPuff.Emit(burstCount);
        }
    }
}
