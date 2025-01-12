using System.Collections;
using UnityEngine;

namespace PlayerSystem
{
    // Use this class to gatekeep powers
    // If the player is not supposed to have a power, then don't instantiate it
    public class PlayerHealthModule : IDamageable
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private HealthUIController healthUIController;
        private Rigidbody2D rb2d;
        private MonoBehaviour mb;

        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        public PlayerHealthModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, HealthUIController healthUIController, MonoBehaviour mb)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.healthUIController = healthUIController;
            this.mb = mb;

            eventBus.Subscribe<RespawnEvent>(Respawn);
        }

        public bool Damage(int damageAmount)
        {
            if (playerState.healthState == HealthState.Stagger) return false;
            if (playerState.activePower != Power.Square)
            {
                CurrentHealth -= damageAmount;
                healthUIController.UpdateHealthUI(CurrentHealth);
                if (CurrentHealth <= 0f)
                {
                    Die();
                    return true;
                }
                eventBus.Publish(new ReceivedDamageEvent());
            }
            return false;
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
            playerState.healthState = HealthState.Death;
            eventBus.Publish(new DeathEvent());
            eventBus.Publish(new PauseEvent());
        }

        public void Respawn(RespawnEvent e)
        {
            mb.StartCoroutine(RespawnSequence());
        }

        private void ResetHealthValues()
        {
            playerState.healthState = HealthState.Undefined;
            CurrentHealth = MaxHealth;
            healthUIController.ResetHealthUI();
        }

        private void SetRespawnPosition()
        {
            Vector3 savedPosition = GameDataManager.Instance.GetSavedPlayerPosition();
            rb2d.position = savedPosition;
        }

        private IEnumerator RespawnSequence()
        {
            yield return mb.StartCoroutine(MenuController.Instance.FadeInSolidPanel());

            ResetHealthValues();
            SetRespawnPosition();

            yield return new WaitForSeconds(0.2f);

            yield return mb.StartCoroutine(MenuController.Instance.FadeOutSolidPanel());

            eventBus.Publish(new UnpauseEvent());
        }
    }
}