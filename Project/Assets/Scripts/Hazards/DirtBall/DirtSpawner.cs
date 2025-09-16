using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject dirtBallPrefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Vector2 direction;
    [SerializeField]
    private float speed;
    [SerializeField]
    public float reloadTime;
    [SerializeField]
    public float startReload = 0;

    [SerializeField] private Animator anim;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private List<GameObject> rocksPrefabs;
    [SerializeField] private List<GameObject> rocksDestroying;
    public Coroutine spawnRoutine;
    public bool start;

    private const float spawnTime = 1;

    private List<GameObject> rocks = new List<GameObject>();

    private const int rocksSpeed = 1;
    private const int maxRocksAmount = 3;

    private void Start()
    {
        if (start) return;
        float startTime = startReload != 0 ? startReload : Random.Range(0.5f, 2.5f);
        SpawnDirtBall(startTime);
    }

    public void StartSpawn()
    {
        float startTime = startReload != 0 ? startReload : Random.Range(0.5f, 2.5f);
        SpawnDirtBall(startTime);
    }

    public void SpawnDirtBall(float time)
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(GenerateDirtBall(time));
    }
    public IEnumerator GenerateDirtBall(float time)
    {
        yield return new WaitForSeconds(time);

        anim?.SetTrigger("Activate");
        particleSystem?.Play();
        StartCoroutine(ThrowRocks());

        yield return new WaitForSeconds(spawnTime);

        GameObject dirtball = Instantiate(dirtBallPrefab);
        dirtball.transform.SetParent(transform);
        dirtball.transform.position = spawnPoint.position;

        DirtBallScript dbScript = dirtball.GetComponent<DirtBallScript>();
        dbScript.spawner = this;
        dbScript.setInitialSpeed(direction, speed);
    }

    private IEnumerator ThrowRocks()
    {
        int randomAmount = Random.Range(1, 4);
        List<GameObject> rocksList = new List<GameObject>();
        for (int i = 0; i < randomAmount; i++)
        {
            GameObject obj = Instantiate(GetRandomPrefab());
            obj.transform.position = new Vector3(spawnPoint.position.x + Random.Range(-0.4f, 0.5f), spawnPoint.position.y);
            float verticalForce = Mathf.Abs(direction.x) > 0 ? Random.Range(0.25f, 0.751f) : 0;
            Vector2 forceDir = new Vector2(direction.x, verticalForce) * rocksSpeed;
            obj.GetComponent<Rigidbody2D>().AddForce(forceDir, ForceMode2D.Impulse);
            rocksList.Add(obj);
            yield return new WaitForSeconds(Random.Range(0, 0.2f));
        }
        AddRocksToTheList(rocksList);
    }

    private GameObject GetRandomPrefab()
    {
        int randomId = Random.Range(0, rocksPrefabs.Count);
        return rocksPrefabs[randomId];
    }

    public void AddRocksToTheList(List<GameObject> newRocks)
    {
        foreach (GameObject newRock in newRocks)
        {
            rocks.Add(newRock);
        }

        if (rocks.Count > maxRocksAmount)
        {
            RemoveRockFromList();
        }
    }

    private void RemoveRockFromList()
    {
        while (rocks.Count > maxRocksAmount)
        {
            GameObject rockToRemove = rocks[0];
            StartCoroutine(FadeAndDestroy(rockToRemove));
            rocks.RemoveAt(0);
        }
    }

    private IEnumerator FadeAndDestroy(GameObject objToBeDestroyed)
    {
        if (objToBeDestroyed == null || rocksDestroying.Contains(objToBeDestroyed)) yield break;

        rocksDestroying.Add(objToBeDestroyed);

        SpriteRenderer sr = objToBeDestroyed?.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            float fadeSpeed = 1f;

            while (sr.color.a > 0)
            {
                float newAlpha = sr.color.a - fadeSpeed * Time.deltaTime;

                Color newColor = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
                sr.color = newColor;

                yield return null;
            }

            Color finalColor = sr.color;
            finalColor.a = 0;
            sr.color = finalColor;

            rocksDestroying.Remove(objToBeDestroyed);
            Destroy(objToBeDestroyed);
        }
    }
}
