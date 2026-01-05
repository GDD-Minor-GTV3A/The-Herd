using Core.Events;

using UnityEngine;

namespace Team_7.Scripts.AI.Events
{
    public class KillSheepEvent : GameEvent
    {
        public GameObject Sheep;
        public KillSheepEvent(GameObject sheep)
        {
            Sheep = sheep;
        }
    }
}
