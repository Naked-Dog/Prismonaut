using System;
using UnityEngine;

public class AnimationEventEmitter : MonoBehaviour
{
    public event Action<string> OnAnimationEventTriggered;

    public void OnAnimationEvent(string eventName)
    {
        OnAnimationEventTriggered?.Invoke(eventName);
    }
}
