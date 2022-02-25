namespace Poseidon.StateMachine
{
    using System;
    
    public interface IStateMachine<T> : IDisposable where T : Enum
    {
        event Action<T> OnStateChanged;
        public State<T> PreviousState { get; }
        public State<T> CurrentState { get; }
        void Run(T initialState);
        void SwitchState(T stateType);
        void SwitchToPrevious();
    }
}