using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Prism : MonoBehaviour, ICollectable, IDropable
{
    public CollectableType CollectableType { get; set; }
    public UnityEvent collect;
    public GameManager gameManager { get; set; }

    [SerializeField] private float disappearTime = 10f;
    [SerializeField] private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        collect.AddListener(gameManager.GetPrism);
    }

    public void Collect()
    {
        collect.Invoke();
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
        if (collision.gameObject.layer == 6)
        {
            rb2d.bodyType = RigidbodyType2D.Static;
        }
    }

    public IEnumerator StartDisappear()
    {
        yield return new WaitForSeconds(disappearTime);
        Destroy(gameObject);
    }

    public void Drop(Transform spawnPoint)
    {
        StartCoroutine(StartDisappear());
        transform.position = spawnPoint.position;
        var force = new Vector2(Random.Range(-3.0f, 3.0f), 8.0f);
        rb2d.AddForce(force, ForceMode2D.Impulse);
    }
}