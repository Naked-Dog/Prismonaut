using System;
using UnityEditor.Experimental.GraphView;
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

            eventBus.Subscribe<OnUpdate>(updateVisuals);
            eventBus.Subscribe<ToggleSquarePowerEvent>(toggleSquarePower);
            eventBus.Subscribe<ToggleTrianglePowerEvent>(toggleTrianglePower);
            eventBus.Subscribe<ToggleCirclePowerEvent>(toggleCirclePower);
            eventBus.Subscribe<OnDamageReceived>(PlayDamageAnimation);
            eventBus.Subscribe<OnDeath>(PlayDeathAnimation);
            eventBus.Subscribe<OnLateUpdate>(DisplayCurrentHelmet);
            eventBus.Subscribe<OnDodge>(OnBeginDodge);

            circleHelmet = Resources.Load<Sprite>("Helmets/Circle_Helmet");
            triangleHelmet = Resources.Load<Sprite>("Helmets/Triangle_Helmet");
            squareHelmet = Resources.Load<Sprite>("Helmets/Square_Helmet");
        }

        private void updateVisuals(OnUpdate e)
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
            animator.SetBool("isUsingSquarePower", playerState.activePower == Power.Square);
            animator.SetBool("isUsingTrianglePower", playerState.activePower == Power.Triangle);
            animator.SetBool("isHurt", playerState.healthState == HealthState.Stagger);
        }

        private void OnBeginDodge(OnDodge e)
        {
            animator.Play("DodgeBegin");
        }

        private void toggleSquarePower(ToggleSquarePowerEvent e)
        {
            playerState.currentPower = Power.Square;
            animator.Play("SquarePower");
        }

        private void toggleTrianglePower(ToggleTrianglePowerEvent e)
        {
            playerState.currentPower = Power.Triangle;
            animator.Play("TrianglePower");
        }

        private void toggleCirclePower(ToggleCirclePowerEvent e)
        {
            playerState.currentPower = Power.Circle;
            animator.Play("CirclePower");
        }

        private void PlayDamageAnimation(OnDamageReceived e)
        {
            string animationName = "";
            switch (playerState.currentPower)
            {
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

        private void PlayDeathAnimation(OnDeath e)
        {
            string animationName = playerState.groundState == GroundState.Grounded ? "Defeat" : "Explode";
            animator.Play(animationName);
        }

        private void DisplayCurrentHelmet(OnLateUpdate e)
        {
            if (playerState.activePower != Power.None || playerState.healthState == HealthState.Stagger) return;

            Sprite currentHelmetSprite = circleHelmet;

            switch (playerState.currentPower)
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