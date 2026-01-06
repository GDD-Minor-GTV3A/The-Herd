using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.AI.Sheep.Event;
using Core.Events;

//using UnityEditor.Search; <- Chris: Do not use UnityEditor on scripts that run in build!

namespace Core.AI.Sheep
{
    [DisallowMultipleComponent]
    public sealed class SheepDistanceTracker : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform _player;

        [Header("Zone Radii")][Min(0f)] [SerializeField]
        private float _safeRadius = 15f;
        [Min(0f)] [SerializeField] private float _warningRadius = 30f;
        [Min(0f)] [SerializeField] private float _dangerousRadius = 45f;
        [Min(0f)] [SerializeField] private float _deathRadius = 60f;

        [Header("Behaviour")] [SerializeField, Min(0.05f)]
        private float _tickInterval = 0.5f;

        [SerializeField] private bool _ignoreStragglers = true;
        
        private readonly Dictionary<SheepStateManager, SheepDistanceZone> _zones = 
            new Dictionary<SheepStateManager, SheepDistanceZone>(32);

        private Coroutine _loop;
        private WaitForSeconds _wait;

        private void Awake()
        {
            if (_player == null)
                _player = transform;

            _warningRadius = Mathf.Max(_warningRadius, _safeRadius);
            _dangerousRadius = Mathf.Max(_dangerousRadius, _warningRadius);
            _deathRadius = Mathf.Max(_deathRadius, _dangerousRadius);
        }

        private void OnEnable()
        {
            if (_tickInterval <= 0f)
                _tickInterval = 0.2f;
            
            _wait = new WaitForSeconds(_tickInterval);
            
            EventManager.AddListener<SheepJoinEvent>(OnSheepJoin);
            EventManager.AddListener<SheepLeaveHerdEvent>(OnSheepLeave);
            EventManager.AddListener<SheepDeathEvent>(OnSheepDeath);
            
            BootstrapSheepList();
            // infinite loop????
            _loop = StartCoroutine(TickLoop());
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SheepJoinEvent>(OnSheepJoin);
            EventManager.AddListener<SheepLeaveHerdEvent>(OnSheepLeave);
            EventManager.AddListener<SheepDeathEvent>(OnSheepDeath);

            if (_loop != null)
            {
                StopCoroutine(_loop);
                _loop = null;
            }
            
            _zones.Clear();
        }

        #region Event handlers

        private void OnSheepJoin(SheepJoinEvent evt)
        {
            if (evt.Sheep == null) return;
            if (_zones.ContainsKey(evt.Sheep)) return;

            _zones[evt.Sheep] = SheepDistanceZone.Unknown;
        }

        private void OnSheepLeave(SheepLeaveHerdEvent evt)
        {
            if (evt.Sheep == null) return;
            _zones.Remove(evt.Sheep);
        }

        private void OnSheepDeath(SheepDeathEvent evt)
        {
            if (evt.Sheep == null) return;
            _zones.Remove(evt.Sheep);
        }

        #endregion

        #region Bootstrap

        private void BootstrapSheepList()
        {
            _zones.Clear();

            if (SheepTracker.Instance != null)
            {
                var ordered = SheepTracker.Instance.GetOrderedSheepList();
                for (int i = 0; i < ordered.Count; i++)
                {
                    var sheep = ordered[i];
                    if (sheep == null || !sheep.isActiveAndEnabled) continue;
                    _zones[sheep] = SheepDistanceZone.Unknown;
                }
            }
            else
            {
                foreach (var sheep in SheepStateManager.AllSheep)
                {
                    if (!sheep || !sheep.isActiveAndEnabled) continue;
                    _zones[sheep] = SheepDistanceZone.Unknown;
                }
            }
        }

        #endregion

        #region CoreLoop

        private IEnumerator TickLoop()
        {
            while (true)
            {
                EvaluateZone();
                yield return _wait;
            }
        }

        private void EvaluateZone()
        {
            if (_player == null) 
                return;

            Vector3 playerPos = _player.position;
            playerPos.y = 0f;
            int safe = 0, warning = 0, dangerous = 0, death = 0, outside = 0, total = 0;

            List<SheepStateManager> toRemove = null;

            foreach (var kvp in _zones)
            {
                SheepStateManager sheep = kvp.Key;
                SheepDistanceZone oldZone = kvp.Value;

                if (sheep == null || !sheep.isActiveAndEnabled)
                {
                    (toRemove ??= new List<SheepStateManager>()).Add(sheep);
                    continue;
                }
                
                if (_ignoreStragglers && sheep.IsStraggler) continue;
                total++;
                Vector3 pos = sheep.transform.position;
                pos.y = 0f;
                float dist = Vector3.Distance(playerPos, pos);

                SheepDistanceZone newZone = GetZoneForDistance(dist);

                switch (newZone)
                {
                    case SheepDistanceZone.Safe: safe++; break;
                    case SheepDistanceZone.Warning: warning++; break;
                    case SheepDistanceZone.Dangerous: dangerous++; break;
                    case SheepDistanceZone.Death: death++; break;
                    case SheepDistanceZone.Outside: outside++; break;
                }

                if (newZone != oldZone)
                {
                    _zones[sheep] = newZone;
                    EventManager.Broadcast(
                        new SheepDistanceZoneChangedEvent(sheep, oldZone, newZone, dist));
                }
            }

            if (toRemove != null)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    _zones.Remove(toRemove[i]);
                }
            }
            
            EventManager.Broadcast(
                new SheepDistanceZonesSummaryEvent(
                    safe, warning, dangerous, death, outside, total));
        }

        private SheepDistanceZone GetZoneForDistance(float distance)
        {
            if (distance <= _safeRadius) return SheepDistanceZone.Safe;
            if (distance <= _warningRadius) return SheepDistanceZone.Warning;
            if (distance <= _dangerousRadius)  return SheepDistanceZone.Dangerous;
            if (distance <= _deathRadius) return SheepDistanceZone.Death;
            return SheepDistanceZone.Outside;
        }
        
        #endregion
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_player == null) return;
            
            Vector3 pos = _player.position;

            UnityEditor.Handles.color = new Color(0.4f, 1f, 0.4f, 1f);
            UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, _safeRadius);

            UnityEditor.Handles.color = new Color(1f, 0.9f, 0.4f, 1f);
            UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, _warningRadius);
            
            UnityEditor.Handles.color = new Color(1f, 0.4f, 0.4f, 1f);
            UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, _dangerousRadius);
            
            UnityEditor.Handles.color = new Color(0.7f, 0.4f, 1f, 1f);
            UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, _deathRadius);
        }
        #endif
    }
}
