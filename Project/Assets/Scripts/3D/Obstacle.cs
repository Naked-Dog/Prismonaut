using UnityEngine;
public class Obstacle : MonoBehaviour
{
    [SerializeField] private FormState figureType = FormState.Circle;
    [SerializeField] private GameObject asteroids;
    [SerializeField] private GameObject asteroidsHollow;
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject[] asteroidFormObjects;
    [SerializeField] private GameObject[] hollowFormObjects;
    [SerializeField] private Material asteroidMaterial;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip breakAsteroid;

    private ObstacleType obstacleType = ObstacleType.Asteroid;

    private void Awake(){
        for (int i = 0; i < asteroidFormObjects.Length; i++)
        {
            asteroidFormObjects[i].SetActive(false);
        }

        for (int i = 0; i < hollowFormObjects.Length; i++)
        {
            hollowFormObjects[i].SetActive(false);
        }
        asteroids.SetActive(false);
        asteroidsHollow.SetActive(false);
        block.SetActive(false);
    }

    public void SetObstacleType(ObstacleType newType){
        obstacleType = newType;
    }

    public void SetFigureType(FormState figure){
        figureType = figure;
        UpdateMesh();
    }

    private void UpdateMesh(){
        switch(obstacleType){
            case ObstacleType.Asteroid:
                asteroidFormObjects[(int)figureType].SetActive(true);
                asteroids.SetActive(true);
            break;
            case ObstacleType.Hollow:
                hollowFormObjects[(int)figureType].SetActive(true);
                asteroidsHollow.SetActive(true);
            break;
            case ObstacleType.Block:
                block.SetActive(true);
            break;
        }
    }

    private void OnTriggerEnter(Collider other){
        Player3DController playerCtrl = other.GetComponent<Player3DController>();
        if(playerCtrl){
            playerCtrl.HitObstacle(figureType, obstacleType);
            checkDestroyObstacle(playerCtrl);
        }

    }

    private void OnTriggerExit(Collider other){
        Player3DController playerCtrl = other.GetComponent<Player3DController>();
        if(playerCtrl){
            if(obstacleType == ObstacleType.Hollow) playerCtrl.PassObstacle();
        }
    }

    private void OnCollisionEnter(Collision other){
        Player3DController playerCtrl = other.transform.GetComponent<Player3DController>();
        if(playerCtrl){
            playerCtrl.HitObstacle(figureType, obstacleType, false);
        }
    }

    private void checkDestroyObstacle(Player3DController player){
        if(player.GetPlayerSate() != PlayerState.Impulse) return;

        if(obstacleType == ObstacleType.Asteroid && player.currentPlayerForm == figureType){
            audioSource.PlayOneShot(breakAsteroid);
            Destroy(gameObject);
        } 
    }
}
