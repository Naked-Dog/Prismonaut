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

    private void Start()
    {
        StartCoroutine(SpawnDirtBall());
    }
    public IEnumerator SpawnDirtBall()
    {
        float randomNum = Random.Range(0.5f, 2.5f);
        Debug.Log("Worked");
        Debug.Log(randomNum);
        yield return new WaitForSeconds(randomNum);
        Debug.Log("Passed");
        GameObject dirtball = Instantiate(dirtBallPrefab);
        dirtball.transform.SetParent(transform);
        dirtball.transform.position = SpawnPoint.position;

        DirtBallScript dbScript = dirtball.GetComponent<DirtBallScript>();
        dbScript.spawner = this;
        dbScript.setInitialSpeed(direction, speed);

        yield return null;
    }
}
