using UnityEngine;
using UnityEngine.Events;

public class Prism : MonoBehaviour, ICollectable
{
    public CollectableType CollectableType { get; set; }
    public UnityEvent collect;
    public GameManager gameManager { get; set; }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Debug.Log(gameManager);
        collect.AddListener(gameManager.GetPrism);
    }

    public void Collect()
    {
        gameObject.SetActive(false);
        collect.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }
}