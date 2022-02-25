namespace Example
{
    using Poseidon.StateMachine;
    using States;
    using UnityEngine;

    public class StateMachineExample : MonoBehaviour
    {
        public ExampleStateType initialState;
        
        private readonly StateMachine<ExampleStateType> exampleStateMachine = new StateMachine<ExampleStateType>(
            new ExampleIdleState(), 
            new ExampleUpdatedState()
            );

        private void OnEnable()
        {
            exampleStateMachine.Run(initialState);
        }

        private void OnDisable()
        {
            exampleStateMachine.Dispose();
        }
    }
}
