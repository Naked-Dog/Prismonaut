using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    // Use this class to gatekeep powers
    // If the player is not supposed to have a power, then don't instantiate it
    public class PlayerHealthModule : IDamageable
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Knockback knockback;
        private Rigidbody2D rb2d;

        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }

        public PlayerHealthModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Knockback knockback)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.knockback = knockback;
            this.rb2d = rb2d;
        }

        public void Damage(float damageAmount, Vector2 hitDirection)
        {
            if (!(playerState.activePower == Power.Square))
            {
                CurrentHealth -= damageAmount;
                Debug.Log("Current Health: " + CurrentHealth);
                if (CurrentHealth <= 0f)
                {
                    Die();
                }
                knockback.CallKnockback(hitDirection, Vector2.up, Input.GetAxisRaw("Horizontal"), rb2d, playerState, damageAmount);
            }
        }

        public void SpikeDamage()
        {
            if (!(playerState.activePower == Power.Square))
            {
                CurrentHealth -= 1f;
                Debug.Log("Current Health: " + CurrentHealth);
                if (CurrentHealth <= 0f)
                {
                    Die();
                }
                else
                {
                    WarpPlayerToSafeGround();
                }
            }
            else
            {
                knockback.CallKnockback(Vector2.up, Vector2.up, Input.GetAxisRaw("Horizontal"), rb2d, playerState, 1);
            }
        }

        public void WarpPlayerToSafeGround()
        {
            rb2d.position = playerState.lastSafeGroundLocation;
        }

        public void Die()
        {
            GameObject.Destroy(rb2d.gameObject);
        }
    }
}