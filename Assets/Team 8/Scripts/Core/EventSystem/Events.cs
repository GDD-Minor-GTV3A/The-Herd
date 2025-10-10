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

    #region AUDIO_EVENTS

    /// <summary>
    /// Event to play a sound effect.
    /// </summary>
    public class PlaySoundEvent : GameEvent
    {
        private string _soundId;

        public string SoundId => _soundId;


        /// <param name="soundId">The ID of the sound to play.</param>
        public PlaySoundEvent(string soundId)
        {
            _soundId = soundId;
        }
    }


    /// <summary>
    /// Event to play background music.
    /// </summary>
    public class PlayMusicEvent : GameEvent
    {
        private string _musicId;

        public string MusicId => _musicId;


        /// <param name="musicId">The ID of the music to play.</param>
        public PlayMusicEvent(string musicId)
        {
            _musicId = musicId;
        }
    }


    /// <summary>
    /// Event to stop the currently playing music.
    /// </summary>
    public class StopMusicEvent : GameEvent
    {
    }


    /// <summary>
    /// Event to set the SFX volume.
    /// </summary>
    public class SetSFXVolumeEvent : GameEvent
    {
        private float _volume;

        public float Volume => _volume;


        /// <param name="volume">The volume level (0-1).</param>
        public SetSFXVolumeEvent(float volume)
        {
            _volume = volume;
        }
    }


    /// <summary>
    /// Event to set the music volume.
    /// </summary>
    public class SetMusicVolumeEvent : GameEvent
    {
        private float _volume;

        public float Volume => _volume;


        /// <param name="volume">The volume level (0-1).</param>
        public SetMusicVolumeEvent(float volume)
        {
            _volume = volume;
        }
    }

    #endregion AUDIO_EVENTS
}