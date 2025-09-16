using PlayerSystem;
using UnityEngine;

public class Spitter : Enemy, IPlayerPowerInteractable
{
    public PlayerSystem.Power weakness;
    public Transform mouthTransform;
    public Transform projectileOrigin;


    private void OnEnable()
    {
        AudioManager.Instance?.Play3DSoundAttached(EnemySpitterSoundsEnum.Idle, transform, true);
    }

    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == weakness)
        {
            Damage(1);
        }
    }

    #region Player Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            bool playerDied = collision.gameObject.GetComponent<PlayerBaseModule>().healthModule.Damage(DamageAmount);
            if (playerDied) return;
            collision.gameObject.GetComponent<PlayerBaseModule>()?.knockback.CallKnockback(direction, Vector2.up, Input.GetAxisRaw("Horizontal"), true);
        }
    }
    #endregion
}
