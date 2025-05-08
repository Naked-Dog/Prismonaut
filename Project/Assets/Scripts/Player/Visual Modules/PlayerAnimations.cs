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

        public PlayerAnimations(EventBus eventBus, PlayerState playerState, Animator animator)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.animator = animator;
            this.movementValues = GlobalConstants.Get<PlayerMovementScriptable>();

            this.eventBus.Subscribe<OnUpdate>(OnUpdate);
        }


        private void OnUpdate(OnUpdate e)
        {
            switch (playerState.activePower)
            {
                case Power.Dodge:
                    SetState(AnimationState.Dodge);
                    return;
                case Power.Drill:
                    animator.transform.parent.rotation = Quaternion.Euler(0, 0, playerState.rotation);
                    return;
                case Power.Shield:
                    SetState(AnimationState.Shield);
                    return;
                default:
                    animator.transform.parent.rotation = Quaternion.identity;
                    break;
            }

            bool isMoving = Mathf.Abs(playerState.velocity.x) > 0.1f;
            bool isGrounded = playerState.groundState == GroundState.Grounded;
            bool isFalling = playerState.velocity.y < -0f;

            // Provisional sprite flipping code
            if (Mathf.Abs(playerState.velocity.x) > 0.05f)
            {
                playerState.facingDirection = playerState.velocity.x > 0 ? Direction.Right : Direction.Left;
                animator.transform.localRotation = Quaternion.Euler(0, playerState.facingDirection == Direction.Left ? 180 : 0, 0);
            }

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
                case AnimationState.Dodge:
                    animator.Play("Dodge");
                    break;
                case AnimationState.Shield:
                    animator.Play("Shield");
                    break;
            }

            currentState = newState;
        }

        public void InvertPlayerFacingDirection()
        {
            playerState.facingDirection = playerState.facingDirection == Direction.Right ? Direction.Left : Direction.Right;
            animator.transform.localRotation = Quaternion.Euler(0, playerState.facingDirection == Direction.Left ? 180 : 0, 0);
        }
    }
}