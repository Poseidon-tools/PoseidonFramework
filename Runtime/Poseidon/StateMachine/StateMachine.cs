using JetBrains.Annotations;

namespace Poseidon.StateMachine
{
    using System;
    using System.Collections.Generic;
    using PlayerLoopUtils;
    using UnityEngine;
    using UpdateInterface;

    /// <inheritdoc />
    public class StateMachine<T> : IStateMachine<T> where T : Enum
    {
        #region Private Variables
        private readonly Dictionary<T, State<T>> states;
        [UsedImplicitly] private State<T>[] statesArray;
        #endregion
        
        #region Public Variables
        public event Action<T> OnStateChanged;
        public State<T> PreviousState { get; private set; }
        public State<T> CurrentState { get; protected set; }
        
        #endregion

        #region Public Methods
        
        public StateMachine(params State<T>[] statesArray)
        {
            states = new Dictionary<T, State<T>>();
            this.statesArray = statesArray;
            foreach (var state in statesArray)
            {
                RegisterState(state);
            }
        }
        
        /// <summary>
        /// Starts the state machine.
        /// </summary>
        public void Run(T initialState = default)
        {
            PlayerLoopRunner.OnPlayerLoopEvent += StateMachinePlayerLoopEventHandler;
            SwitchState(initialState);
        }

        
        public void Dispose()
        {
            PlayerLoopRunner.OnPlayerLoopEvent -= StateMachinePlayerLoopEventHandler;
            CurrentState?.OnExit();
            CurrentState = null; 
        }
        
        public void SwitchState(T stateType)
        {
            if (CurrentState != null)
            {
                if (CurrentState.IsTypeOf(stateType))
                {
                    Debug.Log($"[StateManager] State {stateType} is already running");
                    return;
                }
                CurrentState.OnExit();
            }

            if (!states.ContainsKey(stateType))
            {
                Debug.LogError($"[StatesManager] There is no ({stateType}) state in dictionary.");
                return;
            }

            Debug.Log($"[StateManager] State {stateType} is now active");
            PreviousState = CurrentState;
            CurrentState = states[stateType];

            CurrentState.OnEnter();
            OnStateChanged?.Invoke(CurrentState.StateType);
        }
        
        public void SwitchToPrevious()
        {
            if (PreviousState == null) return;
            SwitchState(PreviousState.StateType);
        }

        #endregion
        
        #region Private Methods

        private void RegisterState(State<T> newState)
        {
            newState.StateMachine = this;
            
            if (states.ContainsKey(newState.StateType))
            {
                Debug.LogError($"[StatesManager] There is already {newState.StateType} state type in dictionary.");
                return;
            }

            states.Add(newState.StateType, newState);
        }

        #endregion

        #region Update Methods
        
        private void StateMachinePlayerLoopEventHandler(PlayerLoopTiming timing)
        {
            switch (timing)
            {
                case PlayerLoopTiming.EarlyUpdate: if (CurrentState is IEarlyUpdate earlyUpdate) earlyUpdate.OnEarlyUpdate(); break;
                case PlayerLoopTiming.FixedUpdate: if (CurrentState is IFixedUpdate fixedUpdate) fixedUpdate.OnFixedUpdate(); break;
                case PlayerLoopTiming.PreUpdate: if (CurrentState is IPreUpdate preUpdate) preUpdate.OnPreUpdate(); break;
                case PlayerLoopTiming.Update: if (CurrentState is IUpdate update) update.OnUpdate(); break;
                case PlayerLoopTiming.PreLateUpdate: if (CurrentState is IPreLateUpdate preLateUpdate) preLateUpdate.OnPreLateUpdate(); break;
                case PlayerLoopTiming.PostLateUpdate: if (CurrentState is IPostLateUpdate postLateUpdate) postLateUpdate.OnPostLateUpdate(); break;
            }
        }

        #endregion
        
    }
}
