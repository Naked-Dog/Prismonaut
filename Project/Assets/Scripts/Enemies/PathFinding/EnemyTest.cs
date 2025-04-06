using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private GridGraph graph;
    [SerializeField] private Transform player;

    protected void Start()
    {
        graph.Scan();
        PathNode start = graph.GetNearestNode(transform.position);
        PathNode goal = graph.GetNearestNode(player.position);
        List<PathNode> path = Pathfinder.FindPath(start, goal);

        if (path != null) {
            foreach (var node in path) {
                Debug.Log($"Paso en: {node.position}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        PathNode start = graph.GetNearestNode(transform.position);
        PathNode goal = graph.GetNearestNode(player.position);
        List<PathNode> path = Pathfinder.FindPath(start, goal);

        if (path != null) {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.color = Color.red;
                if(i+1 < path.Count){
                    Gizmos.DrawLine(path[i].position, path[i+1].position);   
                }
            }
        }
    }


}
