using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericAnimationEvent : MonoBehaviour
{
    public void SetAnimationEvent(int index) => animEvents[index].Invoke();
    [SerializeField] private List<UnityEvent> animEvents;
}
