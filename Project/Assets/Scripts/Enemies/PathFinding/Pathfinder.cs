using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder {
    public static List<PathNode> FindPath(PathNode startNode, PathNode targetNode) {
        PriorityQueue<PathNode> openSet = new();
        HashSet<PathNode> closedSet = new();

        openSet.Enqueue(startNode, 0);
        startNode.gCost = 0;
        startNode.hCost = Vector2.Distance(startNode.position, targetNode.position);
        startNode.cameFrom = null;

        while (openSet.Count > 0) {
            PathNode current = openSet.Dequeue();

            if (current == targetNode) {
                return ReconstructPath(current);
            }

            closedSet.Add(current);

            foreach (var neighbor in current.neighbors) {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                float tentativeG = current.gCost + Vector2.Distance(current.position, neighbor.position);

                if (tentativeG < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Vector2.Distance(neighbor.position, targetNode.position);
                    neighbor.cameFrom = current;

                    if (!openSet.Contains(neighbor)) {
                        openSet.Enqueue(neighbor, neighbor.fCost);
                    }
                }
            }
        }

        return null;
    }

    private static List<PathNode> ReconstructPath(PathNode currentNode) {
        List<PathNode> path = new();

        while (currentNode != null) {
            path.Add(currentNode);
            currentNode = currentNode.cameFrom;
        }

        path.Reverse();
        return path;
    }
}

//This is a priority queue required by the A* algorithm, but can be moved for general use.
public class PriorityQueue<T> {
    private List<(T item, float priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority) {
        elements.Add((item, priority));
    }

    public T Dequeue() {
        int bestIndex = 0;

        for (int i = 1; i < elements.Count; i++) {
            if (elements[i].priority < elements[bestIndex].priority) {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool Contains(T item) {
        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }

    public void Clear() {
        elements.Clear();
    }
}
