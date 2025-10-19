using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float acceleration = 10f;

    private Rigidbody2D rb;
    private float direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        direction = Mathf.Sign(-transform.parent.localScale.x);

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.right * direction * initialSpeed;
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        float newSpeed = rb.linearVelocity.x + direction * acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }
}
