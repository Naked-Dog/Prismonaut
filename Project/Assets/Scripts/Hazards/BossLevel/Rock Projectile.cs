using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float forwardForce = 6f;
    [SerializeField] private float verticalForce = 10f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private float direction;
    private Vector2 launchDirection = Vector2.zero;
    private float forceMultiplier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Transform boss = transform.parent;
        direction = Mathf.Sign(-boss.localScale.x);

        transform.SetParent(null);

        Vector2 baseForce = new Vector2(direction * forwardForce, verticalForce);
        Vector2 finalForce = Quaternion.Euler(0, 0, launchDirection.x) * baseForce * forceMultiplier;

        rb.AddForce(finalForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        spriteRenderer.transform.Rotate(0f, 0f, 90f * direction * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard")) return;
        Destroy(gameObject);
    }

    public void SetLaunchAngle(float angle)
    {
        launchDirection.x = angle;
    }

    public void SetForceMultiplier(float multiplier)
    {
        forceMultiplier = multiplier;
    }
}
