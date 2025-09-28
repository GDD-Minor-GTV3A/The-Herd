using System.Collections.Generic;
using UnityEngine;
// The Game Events used across the Game.
// Anytime there is a need for a new event, it should be added here.

namespace Core.Events
{
    /// <summary>
    /// This class can be used as a container for events that have to be created only once per program.
    /// </summary>
    public static class Events
    {
    }

    //List of all available events.
    #region EVENTS

    /// <summary>
    /// To be removed later.
    /// </summary>
    public class ExampleEvent : GameEvent 
    {
        private string _exampleField;


        public string ExampleField => _exampleField;


        public ExampleEvent(string exampleField) 
        {
            _exampleField = exampleField;
        }
    }

    #endregion EVENTS
    
    
    
    #region QUEST_EVENTS

    public class StartQuestEvent : GameEvent
    {
        private string _questID;

        public string QuestID => _questID;

        public StartQuestEvent(string questID)
        {
            _questID = questID;
        }
    }

    public class CompleteObjectiveEvent : GameEvent
    {
        private string _questID;
        private string _objectiveID;

        public string QuestID => _questID;
        public string ObjectiveID => _objectiveID;

        public CompleteObjectiveEvent(string questID, string objectiveID)
        {
            _questID = questID;
            _objectiveID = objectiveID;
        }
    }
    
    public class QuestCompletedEvent : GameEvent
    {
        private string _questID;

        public string QuestID => _questID;

        public QuestCompletedEvent(string questID)
        {
            _questID = questID;
        }
    }
    
    #endregion QUEST_EVENTS
}