namespace Example.States
{
    using Example;
    using Poseidon.StateMachine;
    using Poseidon.StateMachine.UpdateInterface;
    using UnityEngine;

    public class ExampleUpdatedState : State<ExampleStateType>, IUpdate
    {
        private const float TIME_TO_UPDATE = 1f;
        private float lastUpdatedTime;

        public override ExampleStateType StateType => ExampleStateType.ExampleUpdated;

        public override void OnEnter()
        {
            Debug.Log("Starting ExampleUpdatedState");
        }

        public override void OnExit()
        {
            Debug.Log("ExampleUpdatedState End");
        }

        public void OnUpdate()
        {
            Debug.Log("OnUpdate in one second period");
            if (Time.time - lastUpdatedTime > TIME_TO_UPDATE)
            {
                lastUpdatedTime = Time.time;
                
            }
        }
    }
}