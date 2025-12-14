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
    /// How far is the sheep from the player in zone system
    /// </summary>
    public enum SheepDistanceZone
    {
        Unknown = -1,
        Safe = 0,
        Warning = 1,
        Dangerous = 2,
        Death = 3,
        Outside = 4
    }

    /// <summary>
    /// Type of damage dealt to the sheep might have different effects or interpretation
    /// </summary>
    public enum SheepDamageType
    {
        Basic,
        Scarecrow,
        Enemy1Shooting,
        Drekavac,
        DeathCircle,
        Other
    }
    
    /// <summary>
    /// Notification on sheep changed zones
    /// </summary>
    public sealed class SheepDistanceZoneChangedEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }
        public SheepDistanceZone OldZone { get; }
        public SheepDistanceZone NewZone { get; }
        public float Distance { get; }

        public SheepDistanceZoneChangedEvent(
            SheepStateManager sheep,
            SheepDistanceZone oldZone,
            SheepDistanceZone newZone,
            float distance)
        {
            Sheep = sheep;
            OldZone = oldZone;
            NewZone = newZone;
            Distance = distance;
        }
    }

    /// <summary>
    /// Snapshot of all sheep in each zone
    /// </summary>
    public sealed class SheepDistanceZonesSummaryEvent : GameEvent
    {
        public int SafeCount { get; }
        public int WarningCount { get; }
        public int DangerousCount { get; }
        public int DeathCount { get; }
        public int OutsideCount { get; }
        public int TotalTracked { get; }

        public SheepDistanceZonesSummaryEvent(
            int safeCount,
            int warningCount,
            int dangerousCount,
            int deathCount,
            int outsideCount,
            int totalTracked)
        {
           SafeCount = safeCount;
           WarningCount = warningCount;
           DangerousCount = dangerousCount;
           DeathCount = deathCount;
           OutsideCount = outsideCount;
           TotalTracked = totalTracked;
        }
    }

    /// <summary>
    /// Used by any damage source for sheep
    /// </summary>
    public sealed class SheepDamageEvent : GameEvent
    {
        public SheepStateManager Target { get; }
        public float Amount { get; }
        public Vector3 HitPoint { get; }
        public SheepDamageType DamageType { get; }
        public GameObject Source { get; }

        public SheepDamageEvent(
            SheepStateManager target,
            float amount,
            Vector3 hitPoint,
            SheepDamageType damageType = SheepDamageType.Basic,
            GameObject source = null)
        {
            Target = target;
            Amount = amount;
            HitPoint = hitPoint;
            DamageType = damageType;
            Source = source;
        }
    }

    /// <summary>
    /// Fired by the sheep after the damage was dealt but before sheep dies for any feedback
    /// </summary>
    public sealed class SheepDamagedEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }
        public int OldHealth { get; }
        public int NewHealth { get; }
        public int MaxHealth { get; }
        public float NormalizedHealth => MaxHealth > 0 ? (float)NewHealth / MaxHealth : 0f;

        public SheepDamagedEvent(
            SheepStateManager sheep,
            int oldHealth,
            int newHealth,
            int maxHealth)
        {
            Sheep = sheep;
            OldHealth = oldHealth;
            NewHealth = newHealth;
            MaxHealth = maxHealth;
        }
    }
    
    /// <summary>
    /// Fired when health of the sheep changed possibly for UI
    /// </summary>
    public sealed class SheepHealthChangedEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public float Normalized => MaxHealth > 0 ? (float)CurrentHealth / MaxHealth : 0f;

        public SheepHealthChangedEvent(
            SheepStateManager sheep,
            int currentHealth,
            int maxHealth)
        {
            Sheep = sheep;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }

    /// <summary>
    /// Called when a sheep dies
    /// </summary>
    public class SheepDeathEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }
        public bool CountTowardSanity { get; }

        public SheepDeathEvent(SheepStateManager sheep, bool countTowardSanity = true)
        {
            Sheep = sheep;
            CountTowardSanity = countTowardSanity;
        }
    }

    /// <summary>
    /// Called when a sheep joins the herd
    /// </summary>
    public class SheepJoinEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }
        public bool CountTowardSanity { get; }

        public SheepJoinEvent(SheepStateManager sheep, bool countTowardSanity = true)
        {
            Sheep = sheep;
            CountTowardSanity = countTowardSanity;
        }
    }

    public class SheepLeaveHerdEvent : GameEvent
    {
        public SheepStateManager Sheep { get; }
        
        public bool WasLost { get; }
        public bool Forced { get; }

        public SheepLeaveHerdEvent(SheepStateManager sheep, bool wasLost = false, bool forced = false)
        {
            Sheep = sheep;
            WasLost = wasLost;
            Forced = forced;
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

    public sealed class SheepScareEvent : GameEvent
    {
        public SheepStateManager Target { get; }
        public float Amount { get; }
        public Vector3 SourcePosition { get; }

        public SheepScareEvent(SheepStateManager sheep, float amount, Vector3 sourcePosition)
        {
            Target = sheep;
            Amount = amount;
            SourcePosition = sourcePosition;
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