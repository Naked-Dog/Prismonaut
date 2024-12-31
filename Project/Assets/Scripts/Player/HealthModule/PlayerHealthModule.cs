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
            CurrentHealth -= damageAmount;
            Debug.Log("Current Health: " + CurrentHealth);
            if (CurrentHealth <= 0f)
            {
                Die();
                return;
            }

            //knockback
            knockback.CallKnockback(hitDirection, Vector2.up, Input.GetAxisRaw("Horizontal"), rb2d, playerState, damageAmount);
        }

        public void Die()
        {
            //Die Animation
            Respawn();
        }

        public void Respawn()
        {
            CurrentHealth = MaxHealth;
            Vector3 savedPosition = GameDataManager.Instance.GetSavedPlayerPosition();
            rb2d.position = savedPosition;
        }
    }
}