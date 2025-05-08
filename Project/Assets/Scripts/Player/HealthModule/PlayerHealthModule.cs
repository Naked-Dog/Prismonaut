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
        private Coroutine hpRegenCoroutine;

        public PlayerHealthModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, HealthUIController healthUIController, MonoBehaviour mb)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.healthUIController = healthUIController;
            this.mb = mb;
            eventBus.Subscribe<RequestRespawn>(Respawn);
        }

        public bool Damage(int damageAmount)
        {
            if (playerState.healthState == HealthState.Stagger || playerState.healthState == HealthState.Death) return false;
            if (playerState.activePower != Power.Square)
            {
                if (hpRegenCoroutine != null) mb.StopCoroutine(hpRegenCoroutine);
                playerState.currentHealth -= damageAmount;
                if (playerState.currentHealth <= 0)
                {
                    if (playerState.currentHealthBars == 1)
                    {
                        healthUIController.UpdateHealthUI(0, playerState.healthPerBar, 1);
                        Die();
                        return true;
                    }
                    else
                    {
                        playerState.currentHealthBars--;
                        playerState.currentHealth = playerState.healthPerBar;
                        healthUIController.UpdateCurrentHealthBar(playerState.currentHealthBars);
                    }
                }
                healthUIController.UpdateHealthUI(playerState.currentHealth, playerState.healthPerBar, playerState.currentHealthBars);
                eventBus.Publish(new OnDamageReceived());
            }
            StartHPRegen();
            return false;
        }

        public void SpikeDamage()
        {
            if (playerState.healthState == HealthState.Stagger || playerState.healthState == HealthState.Death) return;
            if (hpRegenCoroutine != null) mb.StopCoroutine(hpRegenCoroutine);

            playerState.currentHealth -= 1;

            healthUIController.UpdateHealthUI(playerState.currentHealth, playerState.healthPerBar, playerState.currentHealthBars);

            if (playerState.currentHealth <= 0)
            {
                if (playerState.currentHealthBars == 1)
                {
                    healthUIController.UpdateHealthUI(0, playerState.healthPerBar, 1);
                    Die();
                    return;
                }
                else
                {
                    playerState.currentHealthBars--;
                    playerState.currentHealth = playerState.healthPerBar;
                    healthUIController.UpdateCurrentHealthBar(playerState.currentHealthBars);
                }
                healthUIController.UpdateHealthUI(playerState.currentHealth, playerState.healthPerBar, playerState.currentHealthBars);
            }
            StartHPRegen();
            WarpPlayerToSafeGround();
        }

        public void WarpPlayerToSafeGround()
        {
            rb2d.position = playerState.lastSafeGroundLocation;
        }

        public void Die()
        {
            healthUIController.SetDeadPortraitImage();
            playerState.healthState = HealthState.Death;
            eventBus.Publish(new OnDeath());
            eventBus.Publish(new RequestPause());
        }

        public void Respawn(RequestRespawn e)
        {
            mb.StartCoroutine(RespawnSequence());
        }

        private void ResetHealthValues()
        {
            playerState.healthState = HealthState.Undefined;
            playerState.currentHealthBars = playerState.MAX_HEALTH_BARS;
            playerState.currentHealth = playerState.healthPerBar;
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

            eventBus.Publish(new RequestUnpause());
        }

        public void StartHPRegen()
        {
            if (playerState.currentHealth >= playerState.healthPerBar)
            {
                hpRegenCoroutine = null;
                return;
            }
            hpRegenCoroutine = mb.StartCoroutine(HPRegen());
        }

        public IEnumerator HPRegen()
        {
            yield return new WaitForSeconds(playerState.hpRegenRate);
            playerState.currentHealth += 1;
            healthUIController.UpdateHealthUI(playerState.currentHealth, playerState.healthPerBar, playerState.currentHealthBars);
            if (playerState.currentHealth < playerState.healthPerBar)
            {
                StartHPRegen();
            }
            else
            {
                hpRegenCoroutine = null;
            }
        }
    }
}