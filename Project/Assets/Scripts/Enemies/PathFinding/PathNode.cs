using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector2 position;
    public bool isWalkable;
    public List<PathNode> neighbors = new List<PathNode>();

    //This is for the A-star data
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;
    public PathNode cameFrom;

    public PathNode(Vector2 position, bool isWalkable)
    {
        this.position = position;
        this.isWalkable = isWalkable;
    }
}
