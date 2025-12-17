// SheepTracker.cs
using System;

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Core.AI.Sheep.Config;
using Core.AI.Sheep.Event;
using Core.Events;


namespace Core.AI.Sheep
{
    public sealed class SheepTracker : MonoBehaviour
    {
        public static SheepTracker Instance {get; private set;}

        private readonly HashSet<SheepStateManager> _aliveSheep = new();
        private readonly List<SheepStateManager> _orderedSheep = new(16);

        private static readonly PersonalityType[] FAMILY_ORDER =
        {
            PersonalityType.Andela, PersonalityType.Sonja, PersonalityType.Ivana, PersonalityType.Nino,
            PersonalityType.Yaro, PersonalityType.Tihomir
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventManager.AddListener<SheepJoinEvent>(OnSheepJoin);
            EventManager.AddListener<SheepLeaveHerdEvent>(OnSheepLeave);
            EventManager.AddListener<SheepDeathEvent>(OnSheepDeath);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<SheepJoinEvent>(OnSheepJoin);
            EventManager.RemoveListener<SheepLeaveHerdEvent>(OnSheepLeave);
            EventManager.RemoveListener<SheepDeathEvent>(OnSheepDeath);
            
            _aliveSheep.Clear();
        }

        private void Start()
        {
            Bootstrap();
        }

        private void Bootstrap()
        {
            foreach (var sheep in SheepStateManager.AllSheep)
            {
                if (!sheep) continue;
                
                // NOTE: We only track sheep that are "in herd" via the join/leave events.
                // Bootstrap does not force-add anything, to avoid changing existing logic.
            }
        }

        private void OnSheepJoin(SheepJoinEvent evt)
        {
            if (evt.Sheep != null)
            {
                // Persist herd sheep across scene loads so we keep the same instances.
                // Note: DontDestroyOnLoad works on root GameObjects.
                var root = evt.Sheep.transform.root.gameObject;
                DontDestroyOnLoad(root);

                _aliveSheep.Add(evt.Sheep);
            }
        }

        private void OnSheepLeave(SheepLeaveHerdEvent evt)
        {
            if (evt.Sheep != null)
            {
                _aliveSheep.Remove(evt.Sheep);
            }
        }

        private void OnSheepDeath(SheepDeathEvent evt)
        {
            if (evt.Sheep != null)
            {
                _aliveSheep.Remove(evt.Sheep);
            }
        }
        
        /// <summary>
        /// Sheep that are currently considered part of the herd (joined + not left + not dead).
        /// </summary>
        public IReadOnlyCollection<SheepStateManager> AliveSheep => _aliveSheep;

        /// <summary>
        /// Moves all alive herd sheep near the given player transform.
        /// This is used after player respawn and after scene transitions.
        /// </summary>
        public void PullAliveSheepTo(Transform player, float radius = 2.5f)
        {
            if (player == null) return;

            int i = 0;
            foreach (var sheep in _aliveSheep)
            {
                if (!sheep) continue;

                // Evenly spread around the player in a circle.
                float angle = (i * 137.5f) * Mathf.Deg2Rad; // golden angle
                float t = Mathf.Clamp01((i % 16) / 15f);
                float r = radius * (0.35f + 0.65f * Mathf.Sqrt(t));
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * r;

                sheep.WarpTo(player.position + offset);
                sheep.SummonToHerd();
                i++;
            }
        }

        public IReadOnlyList<SheepStateManager> GetOrderedSheepList()
        {
            _orderedSheep.Clear();

            // Add the family members in order, if they're alive and exist.
            foreach (var personalityType in FAMILY_ORDER)
            {
                var member = _aliveSheep
                    .FirstOrDefault(s =>
                    {
                        if (!s) return false;
                        var archetype = s.Archetype;
                        if (archetype == null) return false;
                        return archetype.PersonalityType == personalityType;
                    });

                if (member != null)
                    _orderedSheep.Add(member);
            }

            foreach (var sheep in _aliveSheep)
            {
                if (_orderedSheep.Contains(sheep))
                    continue;
                
                var archetype = sheep.Archetype;
                if (archetype == null) continue;
                
                if (archetype.PersonalityType == PersonalityType.Normal)
                    _orderedSheep.Add(sheep);
            }
            
            return _orderedSheep;
        }
    }
}
