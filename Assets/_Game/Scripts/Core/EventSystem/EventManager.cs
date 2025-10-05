using System;
using System.Collections.Generic;


namespace Core.Events
{
    /// <summary>
    /// Parent class for all events.
    /// </summary>
    public abstract class GameEvent
    {
    }


    /// <summary>
    /// Use this manager to subscribe and broadcast events.
    /// </summary>
    public static class EventManager
    {
        static readonly Dictionary<Type, Action<GameEvent>> s_Events = new Dictionary<Type, Action<GameEvent>>();

        static readonly Dictionary<Delegate, Action<GameEvent>> s_EventLookups =
            new Dictionary<Delegate, Action<GameEvent>>();


        /// <summary>
        /// Use this method to add listener to event.
        /// </summary>
        /// <typeparam name="T">Event to add the listener.</typeparam>
        /// <param name="evt">Listener to add.</param>
        public static void AddListener<T>(Action<T> evt) where T : GameEvent
        {
            if (!s_EventLookups.ContainsKey(evt))
            {
                Action<GameEvent> newAction = (e) => evt((T)e);
                s_EventLookups[evt] = newAction;

                if (s_Events.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                    s_Events[typeof(T)] = internalAction += newAction;
                else
                    s_Events[typeof(T)] = newAction;
            }
        }

        /// <summary>
        /// Use this method to remove listener from event.
        /// </summary>
        /// <typeparam name="T">Event to remove the listener.</typeparam>
        /// <param name="evt">Listener to remove.</param>
        public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
        {
            if (s_EventLookups.TryGetValue(evt, out var action))
            {
                if (s_Events.TryGetValue(typeof(T), out var tempAction))
                {
                    tempAction -= action;
                    if (tempAction == null)
                        s_Events.Remove(typeof(T));
                    else
                        s_Events[typeof(T)] = tempAction;
                }

                s_EventLookups.Remove(evt);
            }
        }

        /// <summary>
        /// Use this method to broadcast event for all listeners.
        /// </summary>
        /// <param name="evt">Event to broadcast.</param>
        public static void Broadcast(GameEvent evt)
        {
            if (s_Events.TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);
        }

        /// <summary>
        /// Use this method to remove all listeners from all events.
        /// </summary>
        public static void Clear()
        {
            s_Events.Clear();
            s_EventLookups.Clear();
        }
    }
}