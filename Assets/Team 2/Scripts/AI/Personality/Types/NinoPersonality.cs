using Core.AI.Sheep.Personality;

using UnityEngine;

namespace Core.AI.Sheep.Personality
{
    public sealed class NinoPersonality : BaseSheepPersonality
    {
        public override string PersonalityName => "Nino";

        private const float BASE_SCARE_MULTIPLIER = 0.4f;
        private const float SCARE_STEP = 0.6f;

        private float _scareMultiplier = BASE_SCARE_MULTIPLIER;

        private SheepStateManager _ivana;
        private SheepStateManager _andela;
        private bool _familySearched;
        private bool _ivanaDied;
        private bool _andelaDied;
        
        public NinoPersonality(SheepStateManager sheep) : base(sheep) {}
        
        // ------------------------------------------------------
        // Family search
        // ------------------------------------------------------
        private void FamilyRefs()
        {
            if (_familySearched) return;
            _familySearched = true;

            var all = SheepStateManager.AllSheep;

            for (int i = 0; i < all.Count; i++)
            {
                var s = all[i];
                if (s == null || s == _sheep) continue;

                var p = s.Personality;
                if (p == null) continue;
                
                if (p.PersonalityName == "Ivana")
                    _ivana = s;
                else if (p.PersonalityName == "Andela")
                    _andela = s;
            }
        }

        private void UpdateFamilyStatus()
        {
            FamilyRefs();

            if (!_ivanaDied && (_ivana == null || !_ivana.gameObject.activeInHierarchy))
            {
                _ivanaDied = true;
                _scareMultiplier += SCARE_STEP;
            }

            if (_ivanaDied && !_andelaDied && (_andela == null || !_andela.gameObject.activeInHierarchy))
            {
                _andelaDied = true;
                _scareMultiplier += SCARE_STEP;
            }
        }

        private Vector3 GetFollowAroundSheep(SheepStateManager target, float distance)
        {
            Vector3 targetPos = target.transform.position;
            
            Vector3 dir = _sheep.transform.position - targetPos;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
            {
                dir = -target.transform.forward;
                dir.y = 0f;
            }

            if (dir.sqrMagnitude > 0.0001f)
                dir.Normalize();
            else
                dir = Vector3.back;

            Vector3 basePos = targetPos + dir * distance;

            basePos += Quaternion.Euler(0f, Random.Range(-30f, 30f), 0f)
                       * (Vector3.right * (distance * 0.5f));
            return basePos;
        }

        public override Vector3 GetFollowTarget(SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            UpdateFamilyStatus();

            float baseDistance = sheep.Archetype?.FollowDistance ?? 1.8f;

            if (!_ivanaDied && _ivana != null)
            {
                return GetFollowAroundSheep(_ivana, baseDistance);
            }

            if (!_andelaDied && _andela != null)
            {
                return GetFollowAroundSheep(_andela, baseDistance);
            }
            
            return base.GetFollowTarget(sheep, context);
        }

        public override void OnThreatDetected(Vector3 threatPosition, SheepStateManager sheep, PersonalityBehaviorContext context)
        {
            UpdateFamilyStatus();

            if (_scareMultiplier < 1.0f)
            {
                return;
            }
            
            base.OnThreatDetected(threatPosition, sheep, context);
        }

        protected override Vector3 ComputeEscapeDestination(SheepStateManager sheep, PersonalityBehaviorContext ctx)
        {
            UpdateFamilyStatus();
            
            Vector3 pos = sheep.transform.position;
            if (!ctx.HasThreat || ctx.Threats.Count == 0)
                return pos;
            
            Vector3 flee = Vector3.zero;

            foreach (var t in ctx.Threats)
            {
                if (!t) continue;

                Vector3 away = pos - t.position;
                away.y = 0f;
                
                float d = Mathf.Max(away.magnitude, 0.25f);
                float w = (1f / (d * d)) * _scareMultiplier;

                if (ctx.ThreatRadius.TryGetValue(t, out float r) && r > 0.01f)
                {
                    float boost = Mathf.Clamp01(1f + (r - d) / r);
                    w *= boost;
                }
                
                flee += away.normalized * w;
            }
            
            if (flee.sqrMagnitude < 0.0001f)
                return pos;
            
            flee.Normalize();

            float fleeSeconds = Mathf.Max(sheep.Config?.WalkAwayFromHerdTicks ?? 2f, 1.5f);
            float fleeDist = fleeSeconds * (sheep.Config?.BaseSpeed ?? 2.2f);

            return pos + flee * fleeDist;
        }
    }
}
