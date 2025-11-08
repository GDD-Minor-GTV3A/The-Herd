using Core.Events;

namespace Core.AI.Sheep.Event
{
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
}