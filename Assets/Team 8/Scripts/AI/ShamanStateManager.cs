using System.Collections.Generic;

using Core.Shared.StateMachine;

using UnityEngine;

namespace AI.Shaman
{
    /// <summary>
    /// shaman state machine.
    /// </summary>
    public class ShamanStateManager : StateManager<ShamanState>
    {
        /// <summary>
        /// reference to the shaman component.
        /// </summary>
        public Shaman Shaman { get; private set; }


        /// <summary>
        /// initialization method.
        /// </summary>
        /// <param name="shaman">shaman component.</param>
        public void Initialize(Shaman shaman)
        {
            Shaman = shaman;

            InitializeStatesMap();
            SetState<ShamanIdle>();
        }


        protected override void InitializeStatesMap()
        {
            StatesMap = new Dictionary<System.Type, ShamanState>
            {
                { typeof(ShamanIdle), new ShamanIdle(this) },
                { typeof(ShamanWalkAround), new ShamanWalkAround(this) },
            };
        }
    }
}

