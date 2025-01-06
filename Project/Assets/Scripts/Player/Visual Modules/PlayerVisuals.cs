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
        private SpriteRenderer helmetRender;
        private Sprite circleHelmet;
        private Sprite squareHelmet;
        private Sprite triangleHelmet;


        public PlayerVisuals(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Animator animator, SpriteRenderer helmetRender)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.animator = animator;
            this.helmetRender = helmetRender;

            eventBus.Subscribe<UpdateEvent>(updateVisuals);
            eventBus.Subscribe<ToggleSquarePowerEvent>(toggleSquarePower);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(toggleTrianglePower);
            eventBus.Subscribe<ToggleCirclePowerEvent>(toggleCirclePower);
            eventBus.Subscribe<ReceivedDamageEvent>(updateDamageVisuals);

            circleHelmet = Resources.Load<Sprite>("Helmets/Circle_Helmet");
            triangleHelmet = Resources.Load<Sprite>("Helmets/Triangle_Helmet");
            squareHelmet = Resources.Load<Sprite>("Helmets/Square_Helmet");
        }

        private void updateVisuals(UpdateEvent e)
        {
            bool isMoving = Mathf.Abs(rb2d.velocity.x) > 0.1f;
            bool isGrounded = playerState.groundState == GroundState.Grounded;
            bool isFalling = rb2d.velocity.y < -0.1f;
            int facingHorizontalScale = Math.Sign(rb2d.velocity.x);
            if (isMoving) animator.gameObject.transform.localScale = new Vector3(facingHorizontalScale, 1, 1);
            if (isMoving) playerState.facingDirection = facingHorizontalScale > 0 ? Direction.Right : Direction.Left;
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isFalling", isFalling);
        }

        private void toggleSquarePower(ToggleSquarePowerEvent e)
        {
            helmetRender.sprite = squareHelmet;
            animator.SetBool("isUsingSquarePower", e.toggle);
        }

        private void toggleTrianglePower(ToggleTrianglePowerEvent e)
        {
            helmetRender.sprite = triangleHelmet;
            animator.SetBool("isUsingTrianglePower", e.toggle);
        }

        private void toggleCirclePower(ToggleCirclePowerEvent e)
        {
            helmetRender.sprite = circleHelmet;
            animator.SetBool("isUsingCirclePower", e.toggle);
        }

        private void updateDamageVisuals(ReceivedDamageEvent e)
        {
            
        }
    }
}