using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [SerializeField] private float forwardForce = 6f;
    [SerializeField] private float verticalForce = 10f;

    [Header("Variación de la roca")]
    [SerializeField] private float forwardRandomness = 2f;
    [SerializeField] private float verticalRandomness = 2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Transform boss = transform.parent;
        float direction = Mathf.Sign(-boss.localScale.x);

        transform.SetParent(null);

        float randomForward = forwardForce + Random.Range(-forwardRandomness, forwardRandomness);
        float randomVertical = verticalForce + Random.Range(-verticalRandomness, verticalRandomness);

        Vector2 force = new Vector2(direction * randomForward, randomVertical);
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard")) return;
        Destroy(gameObject);
    }
}
