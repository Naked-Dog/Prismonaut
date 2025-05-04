using System.Collections;
using UnityEngine;

public class DirtSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject dirtBallPrefab;
    [SerializeField]
    private Transform SpawnPoint;
    [SerializeField]
    private Vector2 direction;
    [SerializeField]
    private float speed;
    [SerializeField]
    public float reloadTime;
    public Coroutine spawnRoutine;

    private void Start()
    {
        SpawnDirtBall(Random.Range(0.5f, 2.5f));
    }

    public void SpawnDirtBall(float time)
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(GenerateDirtBall(time));
    }
    public IEnumerator GenerateDirtBall(float time)
    {
        yield return new WaitForSeconds(time);

        GameObject dirtball = Instantiate(dirtBallPrefab);
        dirtball.transform.SetParent(transform);
        dirtball.transform.position = SpawnPoint.position;

        DirtBallScript dbScript = dirtball.GetComponent<DirtBallScript>();
        dbScript.spawner = this;
        dbScript.setInitialSpeed(direction, speed);
    }
}
