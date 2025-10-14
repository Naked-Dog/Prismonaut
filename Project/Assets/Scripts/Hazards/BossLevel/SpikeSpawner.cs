using System.Collections;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour, ICullable
{
    [Header("Spike Prefab")]
    [SerializeField] private GameObject fallingSpikePrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    [SerializeField] private bool enableCulling = false;
    public bool ShouldBeCameraCulled => true;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            SpawnSpike();
        }
    }

    private void SpawnSpike()
    {
        if (fallingSpikePrefab == null) return;

        Instantiate(
            fallingSpikePrefab,
            transform.position,
            transform.rotation
        );
    }
}
