using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpawner : MonoBehaviour
{
    [SerializeField] private bool startOnStart;
    [SerializeField] private bool startOnEnable;
    [SerializeField] public float startReload = 0;
    [SerializeField] private float speed;
    [SerializeField] public float reloadTime;
    [SerializeField] private Animator anim;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private GameObject dirtBallPrefab;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Transform spawnPoint;
    private Coroutine spawnRoutine;

    [SerializeField] private List<GameObject> rocksParticlePrefabs;
    [SerializeField] private List<GameObject> rocksParticlesPool;
    [SerializeField] private List<GameObject> currentRocksParticles = new();
    private List<GameObject> rocksParticlesToBeDestroyed = new();

    private const float spawnTime = 1;
    private const float rocksSpeed = 0.125f;
    private const int maxRocksParticlesAmount = 6;

    private void Start()
    {
        SetPool();
        
        if (!startOnStart) return;

        StartSpawn();
    }


    #region RocksPrefabs

    public void ActivateStartOnEnable() => startOnEnable = true;

    void OnEnable()
    {
        if (!startOnEnable) return;
        StartSpawn();
    }

    public void StartSpawn()
    {
        float startTime = startReload != 0 ? startReload : Random.Range(0.5f, 2.5f);
        SpawnDirtBall(startTime);
    }

    public void SpawnDirtBall(float time)
    {
        if (!isActiveAndEnabled) return;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(GenerateDirtBall(time));
    }

    public IEnumerator GenerateDirtBall(float time)
    {
        yield return new WaitForSeconds(time);

        if (!isActiveAndEnabled) yield break;

        anim?.SetTrigger("Activate");
        particleSystem?.Play();
        AudioManager.Instance?.Play3DSountAtPosition(RocksSounds.PrepareThrow, transform.position);

        if (isActiveAndEnabled) StartCoroutine(ThrowRocks());

        yield return new WaitForSeconds(spawnTime);

        if (!isActiveAndEnabled) yield break;

        GameObject dirtball = Instantiate(dirtBallPrefab);
        dirtball.transform.SetParent(transform);
        dirtball.transform.position = spawnPoint.position;

        DirtBallScript dbScript = dirtball.GetComponent<DirtBallScript>();
        dbScript.spawner = this;
        dbScript.setInitialSpeed(direction, speed);
        AudioManager.Instance?.Play3DSountAtPosition(RocksSounds.ThrowRock, transform.position);
        SpawnDirtBall(reloadTime);
    }
    #endregion

    #region RocksParticles

    private void SetPool()
    {
        for(int i = 0; i < rocksParticlePrefabs.Count; i++)
        {
            GameObject newRock = Instantiate(rocksParticlePrefabs[i]);
            newRock.transform.position = transform.position;
            newRock.transform.SetParent(transform);
            newRock.SetActive(false);
            rocksParticlesPool.Add(newRock);
        }
    }

    private IEnumerator ThrowRocks()
    {
        int randomAmount = Random.Range(1, 4);
        List<GameObject> rocksList = new();

        for (int i = 0; i < randomAmount; i++)
        {
            GameObject rock = GetRockAvailable();

            rock.transform.position = new Vector3(spawnPoint.position.x + Random.Range(-0.4f, 0.5f), spawnPoint.position.y);

            float gravityModifier = direction.y > 0 ? 1 : 0f;
            float verticalForce = Mathf.Abs(direction.x) > 0 ? Random.Range(0.25f, 0.751f) : direction.y * gravityModifier;
            float horizontalForce = direction.x;
            Vector2 forceDir = new Vector2(horizontalForce, verticalForce) * rocksSpeed;

            rock.GetComponent<Rigidbody2D>().AddForce(forceDir, ForceMode2D.Impulse);

            rocksList.Add(rock);

            yield return new WaitForSeconds(Random.Range(0, 0.2f));
        }

        AddRocksParticlesToCurrentList(rocksList);
    }

    public GameObject GetRockAvailable()
    {
        foreach (GameObject rock in rocksParticlesPool)
        {
            if (!rock.activeSelf)
            {
                rock.SetActive(true);
                return rock;
            }
        }
        GameObject newRock = GetRandomPrefab();
        newRock.transform.SetParent(transform);
        rocksParticlesPool.Add(newRock);
        return newRock;
        //CAMBIAR
    }

    private GameObject GetRandomPrefab()
    {
        int randomId = Random.Range(0, rocksParticlePrefabs.Count);
        GameObject newRock = Instantiate(rocksParticlePrefabs[randomId]);
        return newRock;
    }

    public void AddRocksParticlesToCurrentList(List<GameObject> newRocks)
    {
        if (newRocks.Count == 0) return;
        foreach (GameObject newRock in newRocks)
        {
            currentRocksParticles.Add(newRock);
        }

        if (currentRocksParticles.Count > maxRocksParticlesAmount)
        {
            RemoveRockFromList();
        }
    }

    private void RemoveRockFromList()
    {
        while (currentRocksParticles.Count > maxRocksParticlesAmount)
        {
            GameObject rockToReset = currentRocksParticles[0];
            if (isActiveAndEnabled)
            {
                StartCoroutine(FadeAndReset(rockToReset));
            }
            else
            {
                rockToReset.SetActive(false);
                rockToReset.transform.position = transform.position;
            }

            currentRocksParticles.Remove(rockToReset);
        }
    }

    private IEnumerator FadeAndReset(GameObject objToBeReset)
    {
        if (objToBeReset == null || rocksParticlesToBeDestroyed.Contains(objToBeReset)) yield break;

        rocksParticlesToBeDestroyed.Add(objToBeReset);

        SpriteRenderer sr = objToBeReset?.GetComponent<SpriteRenderer>();

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

            objToBeReset.SetActive(false);
            objToBeReset.transform.position = transform.position;
            rocksParticlesToBeDestroyed.Remove(objToBeReset);

            Color finalColor = sr.color;
            finalColor.a = 1;
            sr.color = finalColor;
        }
    }
    #endregion
}
