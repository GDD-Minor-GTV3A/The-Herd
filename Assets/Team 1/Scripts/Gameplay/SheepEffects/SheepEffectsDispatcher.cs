using System.Collections.Generic;
using Core.AI.Sheep.Event;
using Core.Events;

namespace Gameplay.SheepEffects
{
    public static class SheepEffectsDispatcher
    {
        private static List<ISheepEffectsEventsHandler> listeners = new();

        static SheepEffectsDispatcher()
        {
            EventManager.AddListener<SheepJoinEvent>(OnSheepJointHerdEvent);
            EventManager.AddListener<SheepLeaveHerdEvent>(OnSheepLeftHerdEvent);
        }


        public static void AddNewListener(ISheepEffectsEventsHandler newListener)
        {
            listeners.Add(newListener);
        }


        private static void OnSheepJointHerdEvent(SheepJoinEvent evt)
        {
            List<ISheepEffectsEventsHandler> listenersToRemove = new List<ISheepEffectsEventsHandler>();

            foreach (ISheepEffectsEventsHandler listener in listeners)
            {
                if (listener == null)
                {
                    listenersToRemove.Add(listener);
                    continue;
                }

                if (listener.PersonalityType == evt.Sheep.Archetype.PersonalityType)
                    listener.OnSheepJointHerd(evt.Sheep.Archetype);
            }


            foreach (ISheepEffectsEventsHandler listener in listenersToRemove)
                listeners.Remove(listener);
        }


        private static void OnSheepLeftHerdEvent(SheepLeaveHerdEvent evt)
        {
            List<ISheepEffectsEventsHandler> listenersToRemove = new List<ISheepEffectsEventsHandler>();

            foreach (ISheepEffectsEventsHandler listener in listeners)
            {
                if (listener == null)
                {
                    listenersToRemove.Add(listener);
                    continue;
                }

                if (listener.PersonalityType == evt.Sheep.Archetype.PersonalityType)
                    listener.OnSheepLeftHerd(evt.Sheep.Archetype);
            }


            foreach (ISheepEffectsEventsHandler listener in listenersToRemove)
                listeners.Remove(listener);
        }


        public static void Destroy()
        {
            listeners.Clear();
            listeners = null;
            EventManager.RemoveListener<SheepJoinEvent>(OnSheepJointHerdEvent);
        }
    }
}