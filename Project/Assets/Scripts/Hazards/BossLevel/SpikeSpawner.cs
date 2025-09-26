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
    private Coroutine prepareRoutine;

    private void Start()
    {
        if (runAtStart) HandlePrepareSpike();
    }

    private void OnEnable()
    {
        if (runOnEnable) HandlePrepareSpike();
    }

    private void OnDisable()
    {
        if (prepareRoutine != null)
        {
            StopCoroutine(prepareRoutine);
            prepareRoutine = null;
        }
    }

    private IEnumerator PrepareSpike(float startLoadTime = 0)
    {
        yield return new WaitForSeconds(startLoadTime);

        if (currentSpike == null) yield break;

        Animator anim = currentSpike.GetComponent<Animator>();
        if (anim != null) anim.speed = 1;

        for (float t = 0; t < timeToPrepare; t += Time.deltaTime)
        {
            if (currentSpike == null) yield break;

            Vector2 cSpikePos = currentSpike.transform.position;
            currentSpike.transform.position = new Vector2(
                cSpikePos.x,
                Mathf.Lerp(cSpikePos.y, goalPos.position.y, t)
            );
            yield return null;
        }

        Collider2D col = currentSpike.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        if (willFall)
        {
            yield return new WaitForSeconds(Random.Range(minTimeToFall, maxTimeToFall));
            ThrowSpike();
        }
    }

    public void HandlePrepareSpike()
    {
        if (!gameObject.activeInHierarchy) return;

        currentSpike = Instantiate(spikePrefab, transform.position, transform.rotation);
        currentSpike.GetComponent<Collider2D>().enabled = false;
        currentSpike.transform.SetParent(transform);
        currentSpike.transform.position = startPos.position;

        Animator anim = currentSpike.GetComponent<Animator>();
        if (anim != null) anim.speed = 0;

        Spikes spikeComp = currentSpike.GetComponent<Spikes>();
        if (spikeComp != null)
        {
            spikeComp.onSpikeDestroy += () =>
            {
                if (gameObject.activeInHierarchy)
                    HandlePrepareSpike();
            };
        }

        prepareRoutine = StartCoroutine(PrepareSpike(Random.Range(0.5f, 1.5f)));
    }

    private void ThrowSpike()
    {
        if (currentSpike == null) return;

        Rigidbody2D rb = currentSpike.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = gravityScaleOnFall;
        }
    }

    public void HandleHideSpike()
    {
        if (currentSpike == null) return;

        Collider2D col = currentSpike.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (gameObject.activeInHierarchy)
            StartCoroutine(DeactivateAfterFall());
    }

    private IEnumerator DeactivateAfterFall()
    {
        yield return new WaitForSeconds(1.5f);
        if (gameObject != null) gameObject.SetActive(false);
    }
}
