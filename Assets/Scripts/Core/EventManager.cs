using System;
using System.Collections.Generic;
public static class EventManager
{
    private static readonly Dictionary<GameEvent, Action<object>> eventDictionary = new Dictionary<GameEvent, Action<object>>();

    public static void AddListener(GameEvent eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }
    }
    
    public static void RemoveListener(GameEvent eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }
    
    public static void TriggerEvent(GameEvent eventName, object data = null)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(data);
        }
    }
}