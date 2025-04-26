using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PathfindingAgent2D : MonoBehaviour {
        public Transform target;
        public float speed = 3f;
        public bool canFly = false;

        PathfindingGraph2D graph;
        PathFinder2D pathfinder;
        List<Vector2> path;
        int currentWaypoint = 0;

        void Start() {
            graph = FindAnyObjectByType<PathfindingGraph2D>();
            pathfinder = new PathFinder2D(graph);
            RecalculatePath();
        }

        void RecalculatePath() {
            path = pathfinder.FindPath(transform.position, target.position);
            Debug.Log(path);
            currentWaypoint = 0;
        }

        void Update() {
            if (path == null || currentWaypoint >= path.Count) return;

            Vector2 wp = path[currentWaypoint];
            Vector2 dir = (wp - (Vector2)transform.position).normalized;
            transform.Translate(dir * speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, wp) < 0.1f) {
                currentWaypoint++;
            }
        }
    }
}
