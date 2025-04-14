using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector3 worldPosition;
    public bool isWalkable;
    public List<PathNode> neighbors;

    public PathNode(Vector3 worldPosition, bool isWalkable)
    {
        this.worldPosition = worldPosition;
        this.isWalkable = isWalkable;
        neighbors = new List<PathNode>();
    }
}
