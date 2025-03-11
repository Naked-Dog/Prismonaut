using System;
using UnityEngine;

namespace PlayerSystem
{
    public class PlayerAnimations
    {
        private readonly EventBus eventBus;
        private readonly PlayerState playerState;
        private readonly Rigidbody2D rb2d;
        private readonly Animator animator;

        private AnimationState currentState;

        public PlayerAnimations(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Animator animator)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.animator = animator;

            this.eventBus.Subscribe<OnUpdate>(OnUpdate);
        }

        private void OnUpdate(OnUpdate e)
        {
            if (playerState.activePower != Power.None)
            {
                switch (playerState.activePower)
                {
                    case Power.Dodge:
                        SetState(AnimationState.Dodge);
                        break;
                }
                return;
            }

            bool isMoving = Mathf.Abs(rb2d.velocity.x) > 0.1f;
            bool isGrounded = playerState.groundState == GroundState.Grounded;
            bool isFalling = rb2d.velocity.y < -0f;

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
                    animator.Play("DodgeBegin");
                    break;
            }

            currentState = newState;
        }
    }
}