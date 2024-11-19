using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState{
    Playing,
    Win,
    Lose
}

[System.Serializable]
public class Obs
{
    public ObstacleType obstacleType;
    public FormState obstacleForm;
}

[System.Serializable]
public class ObstacleRow {
    public List<Obs> obstaclesList = new();
    public float distanceBetweenObs;
    public float distanceForNextRow;
}

public class Game3DController : MonoBehaviour
{
    [SerializeField] private Player3DController player;
    [SerializeField] private GameObject obstaclePrefab;

    [SerializeField] private GameObject starShip;

    public List<ObstacleRow> rows = new List<ObstacleRow>();

    [SerializeField] private Transform startPoint; 
    public GameState state;

    private int triggerRowCount = 0;

    private void Start(){
        state = GameState.Playing;
        GeneratedRows();
    }

    private void GeneratedRows(){
        Vector3 startPosition = startPoint.position;

        for (int i = 0; i < rows.Count; i++)
        {
            startPosition = new Vector3(-rows[i].distanceBetweenObs, startPosition.y, startPosition.z);

            for (int j = 0; j < rows[i].obstaclesList.Count; j++)
            {
                GameObject newObstacle = Instantiate(obstaclePrefab);
                newObstacle.transform.parent = transform;
                newObstacle.transform.position = new Vector3(startPosition.x + rows[i]. distanceBetweenObs * j, startPosition.y, startPosition.z);
                Obstacle obstacleController = newObstacle.GetComponent<Obstacle>();
                obstacleController.SetObstacleType(rows[i].obstaclesList[j].obstacleType);
                obstacleController.SetFigureType(rows[i].obstaclesList[j].obstacleForm);
            }
            startPosition = new Vector3(0,startPosition.y, startPosition.z + rows[i].distanceForNextRow);
        }

        starShip.transform.position = startPosition;
    }

    // private void RandomizeObstacles(List<Obstacle> obstaclesList){
    //     List<int> figureValues = new List<int>(new int[] { 0, 1, 2 } );

    //     int randomFigure = Random.Range(0, figureValues.Count);
    //     int randomObstacle = Random.Range(0, obstaclesList.Count);
        
    //     obstaclesList[randomObstacle].SetObstacleType(ObstacleType.Hollow);
    //     obstaclesList[randomObstacle].SetFigureType((FormState)randomFigure);

    //     figureValues.Remove(figureValues[randomFigure]);
        
    //     obstaclesList.Remove(obstaclesList[randomObstacle]);

    //     foreach(Obstacle obs in obstaclesList){
    //         int newFigure = Random.Range(0, figureValues.Count);
    //         obs.SetFigureType((FormState)figureValues[newFigure]);
    //         obs.SetObstacleType(ObstacleType.Asteroid);
    //     }
    // }

    public void CheckGameState(bool isDeath)
    {
        if(!isDeath){
            triggerRowCount++;
        }else{
            state = GameState.Lose;
            MenuController.Instance.DisplayLosePanel();
        }
    }
}
