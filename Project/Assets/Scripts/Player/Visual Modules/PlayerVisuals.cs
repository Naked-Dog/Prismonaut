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
            eventBus.Subscribe<ReceivedDamageEvent>(PlayDamageAnimation);
            eventBus.Subscribe<DeathEvent>(PlayDeathAnimation);
            eventBus.Subscribe<LateUpdateEvent>(DisplayCurrentHelmet);

            circleHelmet = Resources.Load<Sprite>("Helmets/Circle_Helmet");
            triangleHelmet = Resources.Load<Sprite>("Helmets/Triangle_Helmet");
            squareHelmet = Resources.Load<Sprite>("Helmets/Square_Helmet");
        }

        private void updateVisuals(UpdateEvent e)
        {
            bool isMoving = Mathf.Abs(rb2d.velocity.x) > 0.1f;
            bool isGrounded = playerState.groundState == GroundState.Grounded;
            bool isFalling = rb2d.velocity.y < -0.1f;
            bool isDeath = playerState.healthState == HealthState.Death;
            int facingHorizontalScale = Math.Sign(rb2d.velocity.x);

            if (isMoving) animator.gameObject.transform.localScale = new Vector3(facingHorizontalScale, 1, 1);
            if (isMoving) playerState.facingDirection = facingHorizontalScale > 0 ? Direction.Right : Direction.Left;

            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isFalling", isFalling);
            animator.SetBool("isDeath", isDeath);
            animator.SetBool("isUsingCirclePower", playerState.activePower == Power.Circle);
            animator.SetBool("isUsingSquarePower", playerState.activePower == Power.Circle);
            animator.SetBool("isUsingTrianglePower", playerState.activePower == Power.Circle);
            animator.SetBool("isHurt", playerState.healthState == HealthState.Stagger);
        }

        private void toggleSquarePower(ToggleSquarePowerEvent e)
        {
            playerState.currentPower = Power.Square;
            helmetRender.sprite = squareHelmet;
            animator.Play("SquarePower");
        }

        private void toggleTrianglePower(ToggleTrianglePowerEvent e)
        {
            playerState.currentPower = Power.Triangle;
            helmetRender.sprite = triangleHelmet;
            animator.Play("TrianglePower");
        }

        private void toggleCirclePower(ToggleCirclePowerEvent e)
        {
            helmetRender.sprite = circleHelmet;
            animator.Play("CirclePower");
        }

        private void PlayDamageAnimation(ReceivedDamageEvent e)
        {
            string animationName = "";
            switch(playerState.currentPower){
                case Power.Circle:
                    animationName = "HurtCircle";
                    break;
                case Power.Square:
                    animationName = "HurtSquare";
                    break;
                case Power.Triangle:
                    animationName = "HurtTriangle";
                    break;
            }
            animator.Play(animationName);
        }

        private void PlayDeathAnimation(DeathEvent e)
        {
            string animationName = playerState.groundState == GroundState.Grounded ? "Defeat" : "Explode";
            animator.Play(animationName);
        }

        private void DisplayCurrentHelmet(LateUpdateEvent e)
        {
            if(playerState.activePower != Power.None || playerState.healthState == HealthState.Stagger) return;

            Sprite currentHelmetSprite = circleHelmet;

            switch(playerState.currentPower)
            {
                case Power.Circle:
                    currentHelmetSprite = circleHelmet;
                    break;
                case Power.Square:
                    currentHelmetSprite = squareHelmet;
                    break;
                case Power.Triangle:
                    currentHelmetSprite = triangleHelmet;
                    break;
            }

            helmetRender.sprite = currentHelmetSprite;
        }
    }
}