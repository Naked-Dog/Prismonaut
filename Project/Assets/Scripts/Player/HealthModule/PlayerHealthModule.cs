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
        private HealthUIController healthUIController;
        private Rigidbody2D rb2d;

        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        public PlayerHealthModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, Knockback knockback, HealthUIController healthUIController)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.knockback = knockback;
            this.rb2d = rb2d;
            this.healthUIController = healthUIController;
        }

        public void Damage(int damageAmount, Vector2 hitDirection)
        {
            if (!(playerState.activePower == Power.Square))
            {
                CurrentHealth -= damageAmount;
                healthUIController.UpdateHealthUI(CurrentHealth);
                Debug.Log("Current Health: " + CurrentHealth);
                if (CurrentHealth <= 0f)
                {
                    Die();
                }
                knockback.CallKnockback(hitDirection, Vector2.up, Input.GetAxisRaw("Horizontal"), rb2d, playerState, damageAmount);
                eventBus.Publish(new ReceivedDamageEvent());
            }
        }

        public void SpikeDamage()
        {
            CurrentHealth -= 1;
            healthUIController.UpdateHealthUI(CurrentHealth);
            Debug.Log("Current Health: " + CurrentHealth);
            if (CurrentHealth <= 0f)
            {
                Die();
                return;
            }
            else
            {
                WarpPlayerToSafeGround();
            }
        }

        public void WarpPlayerToSafeGround()
        {
            rb2d.position = playerState.lastSafeGroundLocation;
        }

        public void Die()
        {
            //Die Animation
            Respawn();
        }

        public void Respawn()
        {
            CurrentHealth = MaxHealth;
            healthUIController.ResetHealthUI();
            Vector3 savedPosition = GameDataManager.Instance.GetSavedPlayerPosition();
            rb2d.position = savedPosition;
        }
    }
}