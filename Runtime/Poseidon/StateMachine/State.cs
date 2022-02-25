namespace Poseidon.StateMachine
{
    using System;

    public abstract class State<T> where T : Enum
    {
        public IStateMachine<T> StateMachine
        {
            protected get;
            set;
        }

        public abstract T StateType { get; }

        #region Public Methods
        
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public bool IsTypeOf(T stateType)
        {
            return StateType.Equals(stateType);
        }
        #endregion
    }
}