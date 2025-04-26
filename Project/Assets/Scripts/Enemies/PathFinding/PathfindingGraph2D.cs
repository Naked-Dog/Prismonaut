using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public class PathfindingGraph2D : MonoBehaviour
    {
        public bool debugDraw = false;

        [Header("Grid Settings")]
        public Vector2 gridOrigin;                // <-- Nuevo: origen en world-space
        public Vector2 gridWorldSize = new Vector2(50,50);
        public float nodeScale = 0.5f;            // escala relativa al cellSize
        public LayerMask obstacleMask;
        public Grid worldGrid;                    // para obtener cellSize

        private PathNode[,] grid;
        private Vector2 nodeDiameter;
        private int gridSizeX, gridSizeY;

        void Awake()
        {
            if (gridOrigin == Vector2.zero)
                gridOrigin = (Vector2)transform.position;

            Scan();
        }

        public void Scan() {
            nodeDiameter = worldGrid.cellSize * nodeScale * 2f;

            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter.x);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter.y);

            grid = new PathNode[gridSizeX, gridSizeY];

            Vector2 bottomLeft = gridOrigin
                                 - new Vector2(gridWorldSize.x, gridWorldSize.y) * 0.5f;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector2 worldPoint = bottomLeft
                                       + new Vector2(
                                           x * nodeDiameter.x + nodeDiameter.x * 0.5f,
                                           y * nodeDiameter.y + nodeDiameter.y * 0.5f
                                         );

                    bool walkable = !Physics2D.OverlapCircle(
                        worldPoint,
                        nodeDiameter.x * 0.5f * 0.9f,
                        obstacleMask
                    );

                    grid[x, y] = new PathNode(walkable, worldPoint, x, y);
                }
            }
        }

        public List<PathNode> GetNeighbours(PathNode node) {
            var neighbours = new List<PathNode>();

            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0) continue;
                    int nx = node.gridX + dx;
                    int ny = node.gridY + dy;
                    if (nx >= 0 && nx < gridSizeX && ny >= 0 && ny < gridSizeY)
                    {
                        neighbours.Add(grid[nx, ny]);
                    }
                }
            }

            return neighbours;
        }

        public PathNode NodeFromWorldPoint(Vector2 worldPos) {
            float percentX = Mathf.Clamp01((worldPos.x 
                            + gridWorldSize.x/2) / gridWorldSize.x);
            float percentY = Mathf.Clamp01((worldPos.y 
                            + gridWorldSize.y/2) / gridWorldSize.y);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
        }

        #if UNITY_EDITOR
        // void OnValidate()
        // {
        //     nodeDiameter = nodeRadius * 2f;
        //     gridSizeX    = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        //     gridSizeY    = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            
        //     Scan();
        // }
        #endif

        private void OnDrawGizmosSelected()
        {
            if (!debugDraw) return;

            if (grid != null) {
                for (int x = 0; x < gridSizeX; x++) {
                    for (int y = 0; y < gridSizeY; y++) {
                        PathNode n = grid[x,y];
                        Gizmos.color = n.walkable ? new Color(1,1,1,0.25f) : new Color(1,0,0,0.5f);
                        Gizmos.DrawCube(n.worldPos, Vector2.one * 0.2f);
                    }
                }
            }
        }
    }
}