using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [SerializeField] private float forwardForce = 6f;
    [SerializeField] private float verticalForce = 8f;
    [SerializeField] private float lifeTime = 5f;

    [Header("Variación de la roca")]
    [SerializeField] private float forwardRandomness = 1f;
    [SerializeField] private float verticalRandomness = 1f;

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

        Destroy(gameObject, lifeTime);
    }
}
