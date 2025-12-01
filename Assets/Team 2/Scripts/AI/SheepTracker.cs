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
                if (!sheep.isActiveAndEnabled) continue;
                
                var health = sheep.GetComponent<SheepHealth>();
                if (health != null && health.IsDead) continue;

                _aliveSheep.Add(sheep);
            }
        }

        private void OnSheepJoin(SheepJoinEvent evt)
        {
            if (evt.Sheep != null)
            {
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
        
        public IReadOnlyList<SheepStateManager> GetOrderedSheepList()
        {
            _orderedSheep.Clear();

            foreach (var type in FAMILY_ORDER)
            {
                SheepStateManager member = null;

                foreach (var sheep in _aliveSheep)
                {
                    var archetype = sheep.Archetype;
                    if (archetype != null && archetype.PersonalityType == type)
                    {
                        member = sheep;
                        break;
                    }
                }
                
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
