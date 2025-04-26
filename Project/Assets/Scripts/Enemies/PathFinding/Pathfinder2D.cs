using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathFinder
{
public class PathFinder2D {
    PathfindingGraph2D graph;

    public PathFinder2D(PathfindingGraph2D graph) {
        this.graph = graph;
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos) {
        PathNode startNode  = graph.NodeFromWorldPoint(startPos);
        PathNode targetNode = graph.NodeFromWorldPoint(targetPos);

        var openSet   = new List<PathNode> { startNode };
        var closedSet = new HashSet<PathNode>();

        foreach (var n in openSet) { 
            n.gCost = n.hCost = 0; 
            n.parent = null; 
        }

        while (openSet.Count > 0) {
            PathNode current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < current.fCost 
                 || (openSet[i].fCost == current.fCost 
                  && openSet[i].hCost < current.hCost))
                    current = openSet[i];

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode) {
                return RetracePath(startNode, targetNode);
            }

            foreach (PathNode neighbour in graph.GetNeighbours(current)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCost = current.gCost 
                            + GetDistance(current, neighbour);
                if (newCost < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    List<Vector2> RetracePath(PathNode start, PathNode end) {
        var path = new List<Vector2>();
        PathNode current = end;

        while (current != start) {
            path.Add(current.worldPos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(PathNode a, PathNode b) {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);

        if (dx > dy) return 14*dy + 10*(dx - dy);
        return 14*dx + 10*(dy - dx);
    }
}
}
