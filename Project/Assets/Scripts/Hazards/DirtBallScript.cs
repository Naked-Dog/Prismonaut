using Unity.VisualScripting;
using UnityEngine;

public class DirtBallScript : MonoBehaviour
{
    [SerializeField]
    private float speedLimit;
    private Rigidbody2D rb;
    public DirtSpawner spawner;
    private float previousSpeedForce;
    private Vector2 direction = Vector2.zero;
    private float speed = 0;
    private bool isHorizontal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);
        previousSpeedForce = isHorizontal ? rb.linearVelocityX : rb.linearVelocityY;
    }

    public void setInitialSpeed(Vector2 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
        rb.linearVelocity = direction * speed;
        isHorizontal = direction.x != 0 ? true : false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Mathf.Abs(previousSpeedForce) > speedLimit)
        {
            Death();
        }
    }

    private void Death()
    {
        StartCoroutine(spawner.SpawnDirtBall());
        Destroy(gameObject, 0.1f);//animation later
    }
}
