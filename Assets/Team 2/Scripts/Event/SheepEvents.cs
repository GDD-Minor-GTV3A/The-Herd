using Core.Events;
using System;
using UnityEngine;

namespace Core.AI.Sheep.Event
{
    /// <summary>
    /// Sanity stages based on percentage
    /// </summary>
    public enum SanityStage
    {
        Stable,        // 100-75%
        Fragile,       // 74-50%
        Unstable,      // 49-25%
        BreakingPoint, // 24-1%
        Death          // 0%
    }

    /// <summary>
    /// Called when a sheep dies
    /// </summary>
    public class SheepDeathEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }

        public SheepDeathEvent(SheepStateManager sheep)
        {
            Sheep = sheep;
        }
    }

    /// <summary>
    /// Called when a sheep joins the herd
    /// </summary>
    public class SheepJoinEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }

        public SheepJoinEvent(SheepStateManager sheep)
        {
            Sheep = sheep;
        }
    }

    /// <summary>
    /// Called when player sanity changes
    /// </summary>
    public class SanityChangeEvent : GameEvent
    {
        public float Percentage { get; }

        public SanityChangeEvent(float percentage)
        {
            Percentage = percentage;
        }
    }

    /// <summary>
    /// Called when the sanity stage changes
    /// </summary>
    public class SanityStageChangeEvent : GameEvent
    {
        public SanityStage OldStage { get; }
        public SanityStage NewStage { get; }

        public SanityStageChangeEvent(SanityStage oldStage, SanityStage newStage)
        {
            OldStage = oldStage;
            NewStage = newStage;
        }
    }

    /// <summary>
    /// Sent by the Player to pet a sheep
    /// <summary>
    public class RequestPetSheepEvent : GameEvent
    {
        public SheepStateManager TargetSheep { get; }

        public RequestPetSheepEvent(SheepStateManager targetSheep)
        {
            TargetSheep = targetSheep;
        }
    }

    /// <summary>
    /// Sending request to UI manager for flashback popup and a callback when it's closed
    /// </summary>
    public class ShowFlashbackEvent : GameEvent
    {
        public Sprite FlashbackImage { get; }
        public Action OnCloseCallback { get; }

        public ShowFlashbackEvent(Sprite image, Action onClose)
        {
            FlashbackImage = image;
            OnCloseCallback = onClose;
        }
    }
}