using UnityEngine;
using UnityEngine.Events;

public class TriggerEventHandler : MonoBehaviour
{
    [System.NonSerialized] public UnityEvent<Collider2D> OnTriggerEnter2DAction = new UnityEvent<Collider2D>();
    [System.NonSerialized] public UnityEvent<Collider2D> OnTriggerStay2DAction = new UnityEvent<Collider2D>();
    [System.NonSerialized] public UnityEvent<Collider2D> OnTriggerExit2DAction = new UnityEvent<Collider2D>();


    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerStay2DAction?.Invoke(other);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter2DAction?.Invoke(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExit2DAction?.Invoke(other);
    }
}
