using Core.AI.Sheep.Personality;
using UnityEngine;

namespace Core.AI.Sheep.Personality
{
    public sealed class IvanaPersonality : BaseSheepPersonality
    {
        public override string PersonalityName => "Ivana";

        public IvanaPersonality(SheepStateManager sheep) : base(sheep) { }
    }
}
