using UnityEngine;

public class TriggerEventHandler : MonoBehaviour
{
    public event System.Action<Collider2D> OnTriggerEnter2DEvent;
    public event System.Action<Collider2D> OnTriggerStay2DEvent;
    public event System.Action<Collider2D> OnTriggerExit2DEvent;


    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerStay2DEvent?.Invoke(other);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter2DEvent?.Invoke(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExit2DEvent?.Invoke(other);
    }
}
