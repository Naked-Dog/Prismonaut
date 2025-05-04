using System;
using System.Collections.Generic;
using UnityEngine;
public class ColliderDetector : MonoBehaviour
{
    private Transform target;

    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        target = other.transform;
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
