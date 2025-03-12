using UnityEngine;

namespace PlayerSystem
{
    public class PlayerAnimations
    {
        private readonly EventBus eventBus;
        private readonly PlayerState playerState;
        private readonly Animator animator;
        private readonly PlayerMovementScriptable movementValues;

        private AnimationState currentState;

        public PlayerAnimations(EventBus eventBus, PlayerState playerState, Animator animator, PlayerMovementScriptable movementValues)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.animator = animator;
            this.movementValues = movementValues;

            this.eventBus.Subscribe<OnUpdate>(OnUpdate);
        }


        private void OnUpdate(OnUpdate e)
        {
            if (playerState.activePower != Power.None)
            {
                switch (playerState.activePower)
                {
                    case Power.Dodge:
                        if (playerState.powerTimeLeft < 0.15f)
                            SetState(AnimationState.DodgeEnd);
                        else SetState(AnimationState.DodgeBegin);
                        break;
                }
                return;
            }

            bool isMoving = Mathf.Abs(playerState.velocity.x) > 0.1f;
            bool isGrounded = playerState.groundState == GroundState.Grounded;
            bool isFalling = playerState.velocity.y < -0f;

            // Provisional sprite flipping code
            playerState.facingDirection = playerState.velocity.x > 0 ? Direction.Right : Direction.Left;
            animator.transform.rotation = Quaternion.Euler(0, playerState.facingDirection == Direction.Left ? 180 : 0, 0);

            if (isGrounded)
            {
                if (isMoving) SetState(AnimationState.Running);
                else SetState(AnimationState.Idle);
            }
            else
            {
                if (isFalling) SetState(AnimationState.Fall);
                else SetState(AnimationState.Jump);
            }
        }

        private void SetState(AnimationState newState)
        {
            if (newState == currentState) return;

            switch (newState)
            {
                case AnimationState.Idle:
                    animator.Play("Idle");
                    break;
                case AnimationState.Running:
                    animator.Play("Run");
                    break;
                case AnimationState.Jump:
                    animator.Play("Jump");
                    break;
                case AnimationState.Fall:
                    animator.Play("JumpFall");
                    break;
                case AnimationState.DodgeBegin:
                    animator.Play("DodgeBegin");
                    break;
                case AnimationState.DodgeEnd:
                    animator.Play("DodgeEnd");
                    break;
            }

            currentState = newState;
        }
    }
}