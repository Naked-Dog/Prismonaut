using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public enum NodeType
    {
        Ground,
        Fly
    }

    public class PathNode
    {
        public bool walkable;
        public Vector2 worldPos;
        public int gridX, gridY;

        // A* costs
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public PathNode parent;

        public PathNode(bool walkable, Vector2 worldPos, int gridX, int gridY) {
            this.walkable = walkable;
            this.worldPos = worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}
