using UnityEngine;
using PlayerSystem;

[RequireComponent(typeof(Collider2D))]
public class HazardBase : MonoBehaviour, ICullable
{
    [Header("Damage Settings")]
    [SerializeField] protected int damage = 1;

    [Header("Hazard Behavior")]
    [SerializeField] protected bool warpPlayer = true;
    [SerializeField] protected bool destroyOnHit = false;
    [SerializeField] protected bool useTrigger = true;

    [SerializeField] private bool enableCulling = false;
    public bool ShouldBeCameraCulled => enableCulling;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;
        HandleCollision(other.gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (useTrigger) return;
        HandleCollision(other.gameObject);
    }

    protected virtual void HandleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerBaseModule>()?.healthModule;
            if (health == null) return;

            health.HazardDamage(damage, warpPlayer);

            OnHitPlayer(other);
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }

    protected virtual void OnHitPlayer(GameObject player)
    {
    }
}
