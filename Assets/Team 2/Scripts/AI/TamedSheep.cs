using System;

using UnityEngine;
using System.Collections.Generic;

namespace Core.AI.Sheep
{
    public class TamedSheep : MonoBehaviour
    {
        public static TamedSheep Instance { get; private set; }
        private readonly HashSet<SheepStateManager> _everTamed = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        public void MarkTamed(SheepStateManager sheep)
        {
            if (!sheep) return;
            _everTamed.Add(sheep);
        }

        public bool WasEverTamed(SheepStateManager sheep)
        {
            return sheep != null && _everTamed.Contains(sheep);
        }
    }
    
}
