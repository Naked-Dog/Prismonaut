using System;
using System.Collections.Generic;

public class EventBus
{
    private Dictionary<Type, List<Action<object>>> _events = new Dictionary<Type, List<Action<object>>>();

    public void Publish<T>(T eventData)
    {
        if (_events.TryGetValue(typeof(T), out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler(eventData);
            }
        }
    }

    public void Subscribe<T>(Action<T> handler)
    {
        if (!_events.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<Action<object>>();
            _events[typeof(T)] = handlers;
        }
        handlers.Add((obj) => handler((T)obj));
    }
}