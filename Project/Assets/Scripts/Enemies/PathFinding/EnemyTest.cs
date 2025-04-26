// using PathFinder;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// public class EnemyTest : MonoBehaviour
// {
//     public Transform player;
//     private PathfindingAgent2D agent;

//     void Start()
//     {
//         agent = GetComponent<PathfindingAgent2D>();
//         agent.SetDestination(player.position);
//     }

//     void Update()
//     {
//         if (agent == null || player == null) return;

//         agent.SetDestination(player.position);
//     }
// }