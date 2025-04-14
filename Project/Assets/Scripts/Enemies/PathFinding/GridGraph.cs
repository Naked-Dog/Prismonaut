using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGraph : MonoBehaviour
{
    public Grid worldGrid; 
    public Tilemap groundTilemap;
    public Tilemap[] staticObstaclesTileMaps;
    public bool debugDraw = true;
    public Dictionary<Vector3, PathNode> nodeMap = new Dictionary<Vector3, PathNode>();

    private Vector3[] directionsToEvaluate = new Vector3[]
    {
        Vector3.left,
        Vector3.up,
        Vector3.right,
        Vector3.down,
        new Vector3(-1, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(-1, -1, 0),
        new Vector3(1, -1, 0)
    };

    private HashSet<Vector3Int> obstacleSet = new HashSet<Vector3Int>();

    public void Scan()
    {
        nodeMap.Clear();
        obstacleSet.Clear();

        BoundsInt bounds = groundTilemap.cellBounds;
        List<Vector3Int> groundCellPositions = new List<Vector3Int>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (groundTilemap.HasTile(cell))
                {
                    groundCellPositions.Add(cell);
                }

                foreach (Tilemap obstacleMap in staticObstaclesTileMaps)
                {
                    if (obstacleMap.HasTile(cell))
                    {
                        obstacleSet.Add(cell);
                        break;
                    }
                }
            }
        }

        GeneratePathNodes(groundCellPositions);
        GenerateConnections();
    }

    private void GeneratePathNodes(List<Vector3Int> groundCellsPositions)
    {
        foreach (Vector3Int cellPos in groundCellsPositions)
        {
            GenerateGroundPathNodes(cellPos);
        }
    }

    private void GenerateGroundPathNodes(Vector3Int cellPos)
    {
        foreach (Vector3 direction in directionsToEvaluate)
        {
            Vector3Int neighborCellPos = cellPos + Vector3Int.FloorToInt(direction);
            if (groundTilemap.HasTile(neighborCellPos) || obstacleSet.Contains(neighborCellPos))
                continue;

            Vector3 neighborWorldPos = worldGrid.CellToWorld(neighborCellPos) + worldGrid.cellSize / 2;
            if (nodeMap.TryGetValue(neighborWorldPos, out var existingNode))
            {
                if (existingNode.isWalkable)
                    continue;
                else
                    existingNode.isWalkable = true;
            }
            else
            {
                nodeMap[neighborWorldPos] = new PathNode(neighborWorldPos, true);
            }
        }
    }

    private void GenerateConnections()
    {
        foreach (PathNode node in nodeMap.Values)
        {
            foreach (Vector3 direction in directionsToEvaluate)
            {
                Vector3Int neighborCellPos = worldGrid.WorldToCell(node.worldPosition) + Vector3Int.FloorToInt(direction);
                Vector3 neighborWorldPos = worldGrid.CellToWorld(neighborCellPos) + worldGrid.cellSize / 2;
                if (nodeMap.TryGetValue(neighborWorldPos, out var neighborNode))
                {
                    node.neighbors.Add(neighborNode);
                }
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (!debugDraw || nodeMap == null)
            return;

        Gizmos.color = Color.green;
        foreach (var node in nodeMap.Values)
        {
            if (node.isWalkable)
                Gizmos.DrawWireCube(node.worldPosition, worldGrid.cellSize);
        }
    }
}
