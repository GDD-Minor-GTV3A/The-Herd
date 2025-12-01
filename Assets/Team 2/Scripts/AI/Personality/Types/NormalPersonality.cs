using UnityEngine;
using System;
using System.Collections.Generic;

using Core.AI.Sheep.Config;
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

        private SheepStateManager _tihomir;
        private bool _tihomirSearched;

        private void GetTihomir()
        {
            if (_tihomirSearched) return;

            var all = SheepStateManager.AllSheep;
            for (int i = 0; i < all.Count; i++)
            {
                var s = all[i];
                if (!s || s == _sheep) continue;

                var archetype = s.Archetype;
                if (archetype == null) continue;

                if (archetype.PersonalityType == PersonalityType.Tihomir)
                {
                    _tihomir = s;
                    break;
                }
            }
            
            _tihomirSearched = true;
        }

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            GetTihomir();

            if (_tihomir != null && _tihomir.isActiveAndEnabled)
            {
                float baseDistance = sheep.Archetype?.FollowDistance ?? 1.8f;
                Vector3 tihomirPos = _tihomir.transform.position;
                
                Vector3 dir = tihomirPos - sheep.transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude < 0.0001f)
                {
                    Vector2 r = Random.insideUnitCircle.normalized;
                    dir = new Vector3(r.x, 0f, r.y);
                }
                
                dir.Normalize();
                
                Vector3 target = tihomirPos - dir * baseDistance;
                target += Quaternion.Euler(0f, Random.Range(-35f, 35f), 0f) * (Vector3.right * (baseDistance * 0.5f));
                return target;
            }
            
            return base.GetFollowTarget(sheep, context);
        }
    }
}