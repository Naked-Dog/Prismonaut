using System;
using UnityEngine;

namespace PlayerSystem
{
    public class Sprite2DAnimatorVisuals : PlayerVisuals
    {
        PlayerState playerState;
        private Animator animator;

        public Sprite2DAnimatorVisuals(EventBus eventBus, PlayerState playerState, Animator animator) : base(eventBus)
        {
            this.animator = animator;

            eventBus.Subscribe<HorizontalMovementEvent>(MoveHorizontally);
            eventBus.Subscribe<JumpMovementEvent>(Jump);
            eventBus.Subscribe<GroundedMovementEvent>(Ground);
            eventBus.Subscribe<UngroundedMovementEvent>(Unground);
        }

        protected override void MoveHorizontally(HorizontalMovementEvent e)
        {
            bool isMoving = Mathf.Abs(e.amount) > 0.1f;
            animator.SetBool("isMoving", isMoving);
            if (isMoving)
            {
                animator.gameObject.transform.localScale = new Vector3(Mathf.Sign(e.amount), 1, 1);
            }
        }

        protected override void Jump(JumpMovementEvent e)
        {
            // Maybe some particles
        }

        protected override void Ground(GroundedMovementEvent e)
        {
            animator.SetBool("isGrounded", true);
        }

        protected override void Unground(UngroundedMovementEvent e)
        {
            animator.SetBool("isGrounded", false);
        }
    }
}