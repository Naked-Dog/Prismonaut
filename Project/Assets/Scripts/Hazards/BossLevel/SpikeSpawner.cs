using System.Collections;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform goalPos;
    [SerializeField] private float timeToPrepare;
    [SerializeField] private bool runAtStart;
    [SerializeField] private bool runOnEnable;
    [SerializeField] private bool willFall;
    [SerializeField] private float minTimeToFall;
    [SerializeField] private float maxTimeToFall;
    [SerializeField] private float gravityScaleOnFall = 1.5f;
    private GameObject currentSpike;

    private void Start()
    {
        if (runAtStart) HandlePrepareSpike();
    }

    void OnEnable()
    {
        if (runOnEnable) HandlePrepareSpike();
    }

    private IEnumerator PrepareSpike(float startLoadTime = 0)
    {
        yield return new WaitForSeconds(startLoadTime);
        currentSpike.GetComponent<Animator>().speed = 1;

        if (currentSpike == null) yield break;

        for (float t = 0; t < timeToPrepare; t += Time.deltaTime)
        {
            Vector2 cSpikePos = currentSpike.transform.position;
            currentSpike.transform.position = new Vector2(cSpikePos.x, Mathf.Lerp(cSpikePos.y, goalPos.position.y, t));
            yield return null;
        }

        currentSpike.GetComponent<Collider2D>().enabled = true;

        if (willFall)
        {
            yield return new WaitForSeconds(Random.Range(minTimeToFall, maxTimeToFall));
            ThrowSpike();
        }
    }

    public void HandlePrepareSpike()
    {
        currentSpike = Instantiate(spikePrefab, transform.position, transform.rotation);
        currentSpike.GetComponent<Collider2D>().enabled = false;
        currentSpike.transform.SetParent(transform);
        currentSpike.transform.position = startPos.position;
        currentSpike.GetComponent<Animator>().speed = 0;
        currentSpike.GetComponent<Spikes>().onSpikeDestroy += HandlePrepareSpike;
        StartCoroutine(PrepareSpike(Random.Range(0.5f, 1.5f)));
    }

    private void ThrowSpike()
    {
        if (currentSpike == null) return;
        Rigidbody2D rb = currentSpike.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScaleOnFall;
    }

    public void HandleHideSpike()
    {
        currentSpike.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(DeactivateAfterFall());
    }

    private IEnumerator DeactivateAfterFall()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}
