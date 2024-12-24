using System;
using UnityEngine;

namespace PlayerSystem
{
    public class PlayerVisuals
    {
        private readonly EventBus eventBus;
        private readonly PlayerState playerState;
        private readonly Rigidbody2D rb2d;
        private readonly Animator animator;

        public PlayerVisuals(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Animator animator)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.animator = animator;

            eventBus.Subscribe<UpdateEvent>(updateVisuals);
            eventBus.Subscribe<ToggleSquarePowerEvent>(toggleSquarePower);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(toggleTrianglePower);
            eventBus.Subscribe<ToggleCirclePowerEvent>(toggleCirclePower);
        }

        private void updateVisuals(UpdateEvent e)
        {
            bool isMoving = Mathf.Abs(rb2d.velocity.x) > 0.1f;
            bool isGrounded = playerState.groundState == GroundState.Grounded;
            int facingHorizontalScale = Math.Sign(rb2d.velocity.x);
            if (isMoving) animator.gameObject.transform.localScale = new Vector3(facingHorizontalScale, 1, 1);
            if (isMoving) playerState.facingDirection = facingHorizontalScale > 0 ? Direction.Right : Direction.Left;
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isGrounded", isGrounded);
        }

        private void toggleSquarePower(ToggleSquarePowerEvent e)
        {
            animator.SetBool("isUsingSquarePower", e.toggle);
        }

        private void toggleTrianglePower(ToggleTrianglePowerEvent e)
        {
            animator.SetBool("isUsingTrianglePower", e.toggle);
        }

        private void toggleCirclePower(ToggleCirclePowerEvent e)
        {
            animator.SetBool("isUsingCirclePower", e.toggle);
        }
    }
}