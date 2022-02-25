namespace Example.States
{
    using Example;
    using Poseidon.StateMachine;

    public class ExampleIdleState : State<ExampleStateType>
    {
        public override ExampleStateType StateType => ExampleStateType.ExampleIdle;

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}