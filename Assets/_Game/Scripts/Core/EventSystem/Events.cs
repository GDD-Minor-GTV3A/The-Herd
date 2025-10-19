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

    /// <summary>
    /// Broadcast when player's square center and size changes.
    /// </summary>
    public sealed class PlayerSquareChangedEvent : GameEvent
    {
        public Vector3 Center { get; }
        public Vector2 HalfExtents { get; }

        public PlayerSquareChangedEvent(Vector3 center, Vector2 halfExtents)
        {
            Center = center;
            HalfExtents = halfExtents;
        }
    }

    ///<summary>
    /// Broadcast on timer to optimize sheep logic to not rely on Update ticks
    ///</summary>
    public sealed class PlayerSquareTickEvent : GameEvent
    {
        public Vector3 Center { get; }
        public Vector2 HalfExtents { get; }

        public PlayerSquareTickEvent(Vector3 center, Vector2 halfExtents)
        {
            Center = center;
            HalfExtents = halfExtents;
        }
    }
    #endregion EVENTS

    #region DOG_EVENTS
    /// <summary>
    /// When player wants to move dog to specific area.
    /// </summary>
    public class DogMoveCommandEvent : GameEvent
    {
        private Vector3 _moveTarget;


        /// <summary>
        /// Where dog should go.
        /// </summary>
        public Vector3 MoveTarget => _moveTarget;


        /// <param name="moveTarget">Where dog should go.</param>
        public DogMoveCommandEvent(Vector3 moveTarget)
        {
            _moveTarget = moveTarget;
        }
    }


    /// <summary>
    /// When player wants dog to follow him.
    /// </summary>
    public class DogFollowCommandEvent : GameEvent
    {
    }
    #endregion DOG_EVENTS



    #region QUEST_EVENTS

    /// <summary>
    /// Event triggered to request the start of a specific quest.
    /// Typically dispatched by gameplay systems when a quest should begin.
    /// </summary>
    public class StartQuestEvent : GameEvent
    {
        /// <summary>
        /// The unique identifier of the quest to start.
        /// </summary>
        private string _questID;

        /// <summary>
        /// Gets the unique ID of the quest being started.
        /// </summary>
        public string QuestID => _questID;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartQuestEvent"/> class.
        /// </summary>
        /// <param name="questID">The unique ID of the quest to start.</param>
        public StartQuestEvent(string questID)
        {
            _questID = questID;
        }
    }

    /// <summary>
    /// Event broadcast when a quest has successfully started.
    /// Typically used by the UI or other systems to display the quest entry.
    /// </summary>
    public class QuestStartedEvent : GameEvent
    {
        /// <summary>
        /// The unique identifier of the started quest.
        /// </summary>
        private string _questID;

        /// <summary>
        /// Gets the unique ID of the quest that was started.
        /// </summary>
        public string QuestID => _questID;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestStartedEvent"/> class.
        /// </summary>
        /// <param name="questID">The unique ID of the quest that has started.</param>
        public QuestStartedEvent(string questID)
        {
            _questID = questID;
        }
    }

    /// <summary>
    /// Event broadcast when an existing quest’s progress changes (e.g., objectives updated).
    /// </summary>
    public class QuestUpdateEvent : GameEvent
    {
        /// <summary>
        /// The unique identifier of the quest being updated.
        /// </summary>
        private string _questID;

        /// <summary>
        /// Gets the unique ID of the quest that was updated.
        /// </summary>
        public string QuestID => _questID;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestUpdateEvent"/> class.
        /// </summary>
        /// <param name="questID">The unique ID of the quest being updated.</param>
        public QuestUpdateEvent(string questID)
        {
            _questID = questID;
        }
    }

    /// <summary>
    /// Event triggered when a specific objective within a quest is completed.
    /// </summary>
    public class CompleteObjectiveEvent : GameEvent
    {
        /// <summary>
        /// The unique identifier of the quest containing the completed objective.
        /// </summary>
        private string _questID;

        /// <summary>
        /// The unique identifier of the completed objective.
        /// </summary>
        private string _objectiveID;

        /// <summary>
        /// Gets the unique ID of the quest containing the completed objective.
        /// </summary>
        public string QuestID => _questID;

        /// <summary>
        /// Gets the unique ID of the completed objective.
        /// </summary>
        public string ObjectiveID => _objectiveID;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteObjectiveEvent"/> class.
        /// </summary>
        /// <param name="questID">The unique ID of the quest containing the objective.</param>
        /// <param name="objectiveID">The unique ID of the completed objective.</param>
        public CompleteObjectiveEvent(string questID, string objectiveID)
        {
            _questID = questID;
            _objectiveID = objectiveID;
        }
    }

    /// <summary>
    /// Event broadcast when a quest is fully completed (all objectives and stages finished).
    /// </summary>
    public class QuestCompletedEvent : GameEvent
    {
        /// <summary>
        /// The unique identifier of the completed quest.
        /// </summary>
        private string _questID;

        /// <summary>
        /// Gets the unique ID of the quest that was completed.
        /// </summary>
        public string QuestID => _questID;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestCompletedEvent"/> class.
        /// </summary>
        /// <param name="questID">The unique ID of the quest that was completed.</param>
        public QuestCompletedEvent(string questID)
        {
            _questID = questID;
        }
    }

    #endregion QUEST_EVENTS
}