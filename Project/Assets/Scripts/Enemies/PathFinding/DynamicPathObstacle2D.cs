// using UnityEngine;

// namespace PathFinder
// {
//     [RequireComponent(typeof(Collider2D))]
//     public class DynamicPathObstacle2D : MonoBehaviour
//     {
//         public PathfindingGraph2D graph;
//         private Collider2D col;
//         private float checkTimer = 0f;

//         [Tooltip("Segundos mínimos entre cada chequeo")]
//         public float checkInterval = 0.5f;
//         [Tooltip("Margen extra alrededor del collider para re-scaneos")]
//         public float margin = 0.5f;

//         private Vector2Int lastCellKey;

//         void Start()
//         {
//             graph = FindAnyObjectByType<PathfindingGraph2D>();
//             if (graph == null)
//             {
//                 Debug.LogError("No se encontró PathfindingGraph2D en escena.");
//                 enabled = false;
//                 return;
//             }
//             col = GetComponent<Collider2D>();
//             lastCellKey = GetCellKey(transform.position);
//         }

//         void Update()
//         {
//             checkTimer += Time.deltaTime;
//             if (checkTimer < checkInterval) return;
//             checkTimer = 0f;

//             var currentKey = GetCellKey(transform.position);
//             if (currentKey != lastCellKey)
//             {
//                 lastCellKey = currentKey;
//                 Bounds bounds = col.bounds;
//                 bounds.Expand(margin);
//                 graph.ScanArea(bounds);
//             }
//         }

//         private Vector2Int GetCellKey(Vector3 pos)
//         {
//             float size = graph.worldGrid.cellSize.x;
//             return new Vector2Int(
//                 Mathf.FloorToInt(pos.x / size),
//                 Mathf.FloorToInt(pos.y / size)
//             );
//         }
//     }
// }

