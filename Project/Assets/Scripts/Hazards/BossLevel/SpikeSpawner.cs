using System.Collections;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour, ICullable
{
    [Header("Spike Prefab")]
    [SerializeField] private GameObject fallingSpikePrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    [Header("Spawner Settings")]
    [SerializeField] private bool loopAutomatic = false;
    [SerializeField] private bool randomDelay = false;
    [SerializeField] private float fixedDelay = 1f;

    [SerializeField] private bool enableCulling = false;
    public bool ShouldBeCameraCulled => enableCulling;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        if (loopAutomatic)
            StartSpawnLoop();
    }

    private void OnDisable()
    {
        StopSpawnLoop();
    }

    public void TriggerSpawn()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnOnce());
    }

    private void StartSpawnLoop()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawnLoop()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnOnce()
    {
        float delay = randomDelay ? Random.Range(minSpawnTime, maxSpawnTime) : fixedDelay;
        yield return new WaitForSeconds(delay);

        SpawnSpike();
        spawnRoutine = null;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = randomDelay ? Random.Range(minSpawnTime, maxSpawnTime) : fixedDelay;
            yield return new WaitForSeconds(delay);

            SpawnSpike();
        }
    }

    private void SpawnSpike()
    {
        if (fallingSpikePrefab == null) return;
        Instantiate(fallingSpikePrefab, transform.position, transform.rotation);
    }
}
