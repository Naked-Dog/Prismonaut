using Unity.VisualScripting;
using UnityEngine;

public class DirtBallScript : MonoBehaviour
{
    [SerializeField]
    private float impulseLimit;
    private Rigidbody2D rb;
    public DirtSpawner spawner;
    private Vector2 direction = Vector2.zero;
    private float speed = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //private void FixedUpdate()
    //{
    //    rb.linearVelocity = direction * speed * Time.fixedDeltaTime * 20;
    //}

    private void Update()
    {
    }

    public void setInitialSpeed(Vector2 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
        rb.linearVelocity = direction * speed;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float normalImpulse = collision.contacts[0].normalImpulse;
        if (normalImpulse > impulseLimit)
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.Log("death");
        spawner.SpawnDirtBall(spawner.reloadTime);
        Destroy(gameObject, 0.1f);//animation later
    }
}
