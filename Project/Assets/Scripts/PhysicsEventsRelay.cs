using UnityEngine;
using UnityEngine.Events;

public class PhysicsEventsRelay : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent<Collider2D> OnTriggerEnter2DAction = new UnityEvent<Collider2D>();
    [System.NonSerialized] public UnityEvent<Collider2D> OnTriggerStay2DAction = new UnityEvent<Collider2D>();
    [System.NonSerialized] public UnityEvent<Collider2D> OnTriggerExit2DAction = new UnityEvent<Collider2D>();
    [System.NonSerialized] public UnityEvent<Collision2D> OnCollisionEnter2DAction = new UnityEvent<Collision2D>();
    [System.NonSerialized] public UnityEvent<Collision2D> OnCollisionStay2DAction = new UnityEvent<Collision2D>();
    [System.NonSerialized] public UnityEvent<Collision2D> OnCollisionExit2DAction = new UnityEvent<Collision2D>();


    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("OnTriggerEnter2D: " + collider.name);
        OnTriggerEnter2DAction?.Invoke(collider);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        OnTriggerStay2DAction?.Invoke(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        OnTriggerExit2DAction?.Invoke(collider);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D: " + collision.collider.name);
        OnCollisionEnter2DAction?.Invoke(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionStay2DAction?.Invoke(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit2DAction?.Invoke(collision);
    }
}
