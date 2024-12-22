using System;
using System.Collections.Generic;
using System.Linq;

// The EventBus is a messaging system that allows classes to subscribe to and publish events.
// These allows classes to remain strictly decoupled from eachother while still being able to communicate.

// Example: 
// ClassA subbsribes one of its methods to eventX
// Later, ClassB publishes eventX
// The method subscribed to eventX is executed

// The events themselves can be structs that carry data
// Meaning that if ClassA needed some parameters to the method subscribed to eventX,
// they can be passed at the time of publishing by ClassB

public class EventBus
{
    private Dictionary<Type, List<Delegate>> _events = new Dictionary<Type, List<Delegate>>();

    public void Publish<T>(T eventData)
    {
        if (_events.TryGetValue(typeof(T), out var handlers))
        {
            // Iterate over a copy of the handlers to avoid modification during iteration
            foreach (var handler in handlers.ToList())
            {
                handler.DynamicInvoke(eventData);
            }
        }
    }

    public void Subscribe<T>(Action<T> handler)
    {
        if (!_events.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<Delegate>();
            _events[typeof(T)] = handlers;
        }

        handlers.Add(handler); // Store the original delegate
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        if (_events.TryGetValue(typeof(T), out var handlers))
        {
            // Remove the specific handler delegate
            handlers.RemoveAll(h => h.Equals(handler));

            // Clean up empty event types
            if (handlers.Count == 0)
            {
                _events.Remove(typeof(T));
            }
        }
    }
}
