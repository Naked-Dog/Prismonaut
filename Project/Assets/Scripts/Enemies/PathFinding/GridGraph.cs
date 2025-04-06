using System;
using System.Collections.Generic;
using UnityEngine;
public class GridGraph : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1;
    public Vector2 gridCenter => transform.position;
    public GameObject pathGuide;
    public LayerMask obstacle;
    public bool drawGrid = false;

    private PathNode[,] pathMap;

    public void Scan(){
        GenerateGridPositions();
        GenerateNodeConnections();
    }

    public void GenerateGridPositions()
    {
        pathMap = new PathNode[width, height];
        float initialXpos = gridCenter.x - (width / 2f) * cellSize;
        float initialYpos = gridCenter.y - (height / 2f) * cellSize;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                Vector2 nodePosition = new Vector2(initialXpos + (i * cellSize), initialYpos + (j * cellSize));
                Vector2 nodeSize = Vector2.zero;
                bool isNodeWalkable = IsPathNodeWalkable(nodePosition, obstacle);
                pathMap[i,j] = new PathNode(nodePosition, isNodeWalkable);
            }
        }
    }

    private bool IsPathNodeWalkable(Vector2 nodePosition, LayerMask mask)
    {
         return !Physics2D.OverlapCircle(nodePosition, cellSize, mask);
    }

    private void GenerateNodeConnections()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                PathNode currentNode = pathMap[i,j];

                if (currentNode.isWalkable)
                {
                    AddNeighborConnection(currentNode, i + 1, j);
                    AddNeighborConnection(currentNode, i - 1, j);
                    AddNeighborConnection(currentNode, i, j + 1);
                    AddNeighborConnection(currentNode, i, j - 1);
                    AddNeighborConnection(currentNode, i + 1, j + 1);
                    AddNeighborConnection(currentNode, i - 1, j - 1);
                    AddNeighborConnection(currentNode, i + 1, j - 1);
                    AddNeighborConnection(currentNode, i - 1, j + 1);
                }
            }
        }
    }

    private void AddNeighborConnection(PathNode currentNode, int neighborX, int neighborY)
    {
        if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
        {
            PathNode neighborNode = pathMap[neighborX, neighborY];

            if (neighborNode.isWalkable)
            {
                currentNode.neighbors.Add(neighborNode);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGrid || pathMap == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                PathNode node = pathMap[i, j];

                if (!node.isWalkable)
                {
                    Gizmos.DrawCube(node.position, Vector3.one * cellSize * 0.4f);
                }
            }
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                PathNode node = pathMap[i, j];

                if (node.isWalkable)
                {
                    foreach (var neighbor in node.neighbors)
                    {
                        if (neighbor.position.x >= node.position.x && neighbor.position.y >= node.position.y)
                        {
                            Gizmos.DrawLine(node.position, neighbor.position);
                        }
                    }
                }
            }
        }
    }

    public PathNode GetNearestNode(Vector2 worldPosition)
    {
        float initialXpos = gridCenter.x - (width / 2f) * cellSize;
        float initialYpos = gridCenter.y - (height / 2f) * cellSize;

        int x = Mathf.RoundToInt((worldPosition.x - initialXpos) / cellSize);
        int y = Mathf.RoundToInt((worldPosition.y - initialYpos) / cellSize);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return pathMap[x,y];
        }

        return null;
    }
}
