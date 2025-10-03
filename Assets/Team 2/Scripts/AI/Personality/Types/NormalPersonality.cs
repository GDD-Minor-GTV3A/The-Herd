using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Shared.StateMachine;

using Random = UnityEngine.Random;

namespace Core.AI.Sheep.Personality.Types
{
    /// <summary>
    /// Normal personality - implements the default sheep behavior
    /// This contains all the logic that was previously in SheepStateManager
    /// </summary>
    public class NormalPersonality : BaseSheepPersonality
    {
        public NormalPersonality(SheepStateManager sheep) : base(sheep) { }

        public override string PersonalityName => "Normal";


    }
}