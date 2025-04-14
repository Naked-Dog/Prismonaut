using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    private Dictionary<Vector3, PathNode> nodeMap;

    public Pathfinder(Dictionary<Vector3, PathNode> nodeMap)
    {
        this.nodeMap = nodeMap;
    }


    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos, AgentProperties agentProps)
    {
        PathNode startNode = GetClosestNode(startWorldPos);
        PathNode endNode = GetClosestNode(endWorldPos);

        if (startNode == null || endNode == null)
            return null;

        var openSet = new PriorityQueue<PathNode>();
        var cameFrom = new Dictionary<PathNode, PathNode>();
        var gScore = new Dictionary<PathNode, float>();
        var fScore = new Dictionary<PathNode, float>();

        foreach (var node in nodeMap.Values)
        {
            gScore[node] = float.PositiveInfinity;
            fScore[node] = float.PositiveInfinity;
        }
        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, endNode);

        openSet.Enqueue(startNode, fScore[startNode]);

        while (openSet.Count > 0)
        {
            PathNode current = openSet.Dequeue();

            if (current == endNode || Vector3.Distance(current.worldPosition, endNode.worldPosition) < 0.001f)
            {
                return ReconstructPath(cameFrom, current);
            }
            
            foreach (PathNode neighbor in current.neighbors)
            {
                if (!IsValidTransition(current, neighbor, agentProps))
                    continue;

                float tentativeG = gScore[current] + Vector3.Distance(current.worldPosition, neighbor.worldPosition);

                if (tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, endNode);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                    else
                    {
                        openSet.UpdatePriority(neighbor, fScore[neighbor]);
                    }
                }
            }
        }
        return null;
    }
    private bool IsValidTransition(PathNode current, PathNode neighbor, AgentProperties props)
    {
        float yDiff = neighbor.worldPosition.y - current.worldPosition.y;
        if (yDiff > props.maxClimbHeight)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(current.worldPosition, neighbor.worldPosition, props.groundLayer);
        if (hit.collider != null)
            return false;

        return true;
    }

    private PathNode GetClosestNode(Vector3 position)
    {
        float minDist = float.MaxValue;
        PathNode closest = null;
        foreach (var node in nodeMap.Values)
        {
            if (!node.isWalkable)
                continue;

            float dist = Vector3.Distance(position, node.worldPosition);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }
        return closest;
    }

    private float Heuristic(PathNode a, PathNode b)
    {
        return Vector3.Distance(a.worldPosition, b.worldPosition);
    }

    private List<Vector3> ReconstructPath(Dictionary<PathNode, PathNode> cameFrom, PathNode current)
    {
        var totalPath = new List<Vector3> { current.worldPosition };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current.worldPosition);
        }
        return totalPath;
    }
}

public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].priority < elements[bestIndex].priority)
                bestIndex = i;
        }
        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public void UpdatePriority(T item, float newPriority)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(elements[i].item, item))
            {
                elements[i] = (item, newPriority);
                break;
            }
        }
    }

    public bool Contains(T item)
    {
        return elements.Any(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }
}
