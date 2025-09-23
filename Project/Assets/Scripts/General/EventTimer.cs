using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventTimer : MonoBehaviour
{
    [SerializeField] private UnityEvent EventAfterTimer;
    [SerializeField] private float timeToSet = 1;

    public void SetTimedEvent()
    {
        StartCoroutine(TimedEvent());
    }

    private IEnumerator TimedEvent()
    {
        yield return new WaitForSeconds(timeToSet);
        EventAfterTimer?.Invoke();
    }
}
