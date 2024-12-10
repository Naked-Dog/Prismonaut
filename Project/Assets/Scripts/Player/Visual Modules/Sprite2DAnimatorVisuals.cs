using System;
using UnityEngine;

namespace PlayerSystem
{
    public class Sprite2DAnimatorVisuals : PlayerVisuals
    {
        PlayerState playerState;
        Rigidbody2D rb2d;
        private Animator animator;

        public Sprite2DAnimatorVisuals(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Animator animator) : base(eventBus)
        {
            this.rb2d = rb2d;
            this.animator = animator;

            eventBus.Subscribe<HorizontalMovementEvent>(MoveHorizontally);
            eventBus.Subscribe<JumpMovementEvent>(Jump);
            eventBus.Subscribe<GroundedMovementEvent>(Ground);
            eventBus.Subscribe<UngroundedMovementEvent>(Unground);
            eventBus.Subscribe<UpdateEvent>(updateThings);
        }

        protected override void MoveHorizontally(HorizontalMovementEvent e)
        {
            // bool isMoving = Mathf.Abs(e.amount) > 0.1f;
            // animator.SetBool("isMoving", isMoving);
            // if (isMoving)
            // {
            //     animator.gameObject.transform.localScale = new Vector3(Mathf.Sign(e.amount), 1, 1);
            // }
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

        private void updateThings(UpdateEvent e)
        {
            bool isMoving = Mathf.Abs(rb2d.velocity.x) > 0.1f;
            animator.SetBool("isMoving", isMoving);
            if (isMoving) animator.gameObject.transform.localScale = new Vector3(Mathf.Sign(rb2d.velocity.x), 1, 1);
        }
    }
}