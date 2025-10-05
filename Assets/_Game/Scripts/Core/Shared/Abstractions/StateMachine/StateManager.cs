using System;
using System.Collections.Generic;

using UnityEngine;

namespace Core.Shared.StateMachine
{
    /// <summary>
    /// Use this class as a base for state manager for static objects.
    /// </summary>
    /// <typeparam name="T">State class for this manager.</typeparam>
    public abstract class StateManager<T> : MonoBehaviour where T : IState
    {
        protected T _currentState;

        /// <summary>
        /// Map of all states.
        /// </summary>
        public Dictionary<Type, T> StatesMap { get; protected set; }


        protected abstract void InitializeStatesMap();


        /// <summary>
        /// Use this method to start new state.
        /// </summary>
        /// <typeparam name="U">State to start.</typeparam>
        public virtual void SetState<U>() where U : T
        {
            if (_currentState != null)
                _currentState.OnStop();

            _currentState = StatesMap[typeof(U)];
            _currentState.OnStart();
        }


        protected virtual void Update()
        {
            if (_currentState != null)
                _currentState.OnUpdate();
        }
    }
}