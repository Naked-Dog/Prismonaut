using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    private float targetDistance;
    
    void Start()
    {
        followTarget = GameObject.FindGameObjectWithTag("Player").transform;

        targetDistance = Vector3.Distance(followTarget.position, transform.position);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, followTarget.position.z - targetDistance);
    }
}
