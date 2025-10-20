using DG.Tweening;
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
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector2.one, 1f).SetEase(Ease.OutBack);
        direction = Mathf.Sign(-transform.parent.localScale.x);
        rb.linearVelocity = Vector2.right * direction * initialSpeed;
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        float newSpeed = rb.linearVelocity.x + direction * acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
