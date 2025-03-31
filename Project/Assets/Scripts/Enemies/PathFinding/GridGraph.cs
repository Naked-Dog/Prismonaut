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

    private List<PathNode> pathMap = new();

    public void Scan(){
        GenerateGridPositions();
        GenerateNodeConnections();
    }

    public void GenerateGridPositions()
    {
        pathMap.Clear();

        float initialXpos = gridCenter.x - (width / 2f) * cellSize;
        float initialYpos = gridCenter.y - (height / 2f) * cellSize;

        for (int i = 0; i <= width; i++)
        {
            for (int j = 0; j <= height; j++) 
            {
                Vector2 nodePosition = new Vector2(initialXpos + (i * cellSize), initialYpos + (j * cellSize));
                Vector2 nodeSize = Vector2.zero;
                bool isNodeWalkable = IsPathNodeWalkable(nodePosition, nodeSize, obstacle);
                pathMap.Add(new PathNode(nodePosition, nodeSize, isNodeWalkable));
            }
        }
    }

    private bool IsPathNodeWalkable(Vector2 nodePosition, Vector2 nodeSize, LayerMask mask)
    {
        float cellOffset = cellSize/2f;
        Vector2[] checkPoints = {
            nodePosition + new Vector2(cellOffset, 0),
            nodePosition + new Vector2(-cellOffset, 0),
            nodePosition + new Vector2(0, cellOffset),
            nodePosition + new Vector2(0, -cellOffset),
        };

        foreach(Vector2 point in checkPoints){
            Collider2D hit = Physics2D.OverlapCircle(point, 0.1f, mask);
            if(hit){
                return false;
            }
        }
        return true;
}

    private void GenerateNodeConnections()
    {
        for (int i = 0; i <= width; i++)
        {
            for (int j = 0; j <= height; j++)
            {
                int currentNodeIndex = j * (width + 1) + i;
                PathNode currentNode = pathMap[currentNodeIndex];

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
        if (neighborX >= 0 && neighborX <= width && neighborY >= 0 && neighborY <= height)
        {
            int neighborIndex = neighborY * (width + 1) + neighborX;
            PathNode neighborNode = pathMap[neighborIndex];

            if (neighborNode.isWalkable)
            {
                currentNode.neighbors.Add(neighborNode);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pathMap.Count == 0) return;

        if(drawGrid){
            Gizmos.color = Color.red;
            foreach(PathNode node in pathMap){
                if(!node.isWalkable){
                    Gizmos.DrawSphere(node.position, 0.1f);
                }
            }

            Gizmos.color = Color.blue;
            foreach(PathNode node in pathMap){
                if(node.isWalkable && node.neighbors != null){
                    foreach(PathNode neighbor in node.neighbors){
                        Gizmos.DrawLine(node.position, neighbor.position);
                    }
                }
            }
        }
    }
}

public class PathNode
{
    public Vector2 position;
    public Vector2 size;
    public bool isWalkable;
    public List<PathNode> neighbors = new List<PathNode>();

    public PathNode(Vector2 position, Vector2 size, bool isWalkable)
    {
        this.position = position;
        this.size = size;
        this.isWalkable = isWalkable;
    }
}
