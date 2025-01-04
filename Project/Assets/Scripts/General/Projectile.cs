using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Collider2D EnemyCollider { get; private set; }
    public Vector2 Direction { get; private set; }
    [SerializeField] public int damageAmount = 1;
    [SerializeField] private bool isDestroyedByTerrain = true;

    private Collider2D coll;
    private Rigidbody2D rb;
    private IDamageable iDamageable;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        IgnoreCollisionWithEnemyToggle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        iDamageable = collision.gameObject.GetComponent<IDamageable>();
        if (iDamageable != null && !collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamageable>()?.Damage(damageAmount, transform.up);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerBaseModule>()?.healthModule.Damage(damageAmount, Direction);
            GameObject.Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isDestroyedByTerrain) GameObject.Destroy(this.gameObject);
        }
    }

    private void IgnoreCollisionWithEnemyToggle()
    {
        if (!Physics2D.GetIgnoreCollision(coll, EnemyCollider))
        {
            Physics2D.IgnoreCollision(coll, EnemyCollider, true);
        }
        else
        {
            Physics2D.IgnoreCollision(coll, EnemyCollider, false);
        }
    }

    public void Initialize(Collider2D enemyCollider, Vector2 direction)
    {
        EnemyCollider = enemyCollider;
        Direction = direction;
    }
}
