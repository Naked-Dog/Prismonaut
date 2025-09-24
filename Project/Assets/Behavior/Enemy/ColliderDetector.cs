using System.Collections.Generic;
using UnityEngine;
public class ColliderDetector : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private List<string> tagList = new List<string>();

    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var tag in tagList)
        {
            if (other.CompareTag(tag))
            {
                target = other.transform;
                break;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        target = null;
    }

    public Transform GetTarget(string tag)
    {
        if (target != null && target.CompareTag(tag))
        {
            return target;
        }
        return null;
    }
}
