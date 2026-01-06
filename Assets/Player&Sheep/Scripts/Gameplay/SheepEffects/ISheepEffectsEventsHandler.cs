using Core.AI.Sheep.Config;

namespace Gameplay.SheepEffects
{
    public interface ISheepEffectsEventsHandler
    {
        public PersonalityType PersonalityType { get; }


        public void OnSheepJointHerd(SheepArchetype archetype);
        public void OnSheepLeftHerd(SheepArchetype archetype);
    }
}
