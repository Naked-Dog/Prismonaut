using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyTest : MonoBehaviour
{
    [SerializeField] private Transform player;
    private PathFinder2DAgent pathFinderAgent;

    void Start()
    {
        pathFinderAgent = GetComponent<PathFinder2DAgent>();
        pathFinderAgent.TargetDestination = player.position;
    }
}
