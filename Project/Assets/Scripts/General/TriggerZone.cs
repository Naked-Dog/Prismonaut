using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

    [SerializeField] private string collisionTag = "Player";

    [SerializeField] private bool isOneShot;
    [SerializeField] private bool alreadyEntered;
    [SerializeField] private bool alreadyExited;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyEntered) return;

        if (!string.IsNullOrEmpty(collisionTag) && !collision.CompareTag(collisionTag)) return;

        onTriggerEnter?.Invoke();
        if(isOneShot) alreadyEntered = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (alreadyExited) return;

        if (!string.IsNullOrEmpty(collisionTag) && !collision.CompareTag(collisionTag)) return;

        onTriggerExit?.Invoke();
        if (isOneShot) alreadyExited = true;
    }
}
