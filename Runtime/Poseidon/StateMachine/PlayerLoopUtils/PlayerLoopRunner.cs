namespace Poseidon.StateMachine.PlayerLoopUtils
{
    using System;

    internal sealed class PlayerLoopRunner
    {
        public static event Action<PlayerLoopTiming> OnPlayerLoopEvent;

        private readonly PlayerLoopTiming timing;

        public PlayerLoopRunner(PlayerLoopTiming timing)
        {
            this.timing = timing;
        }
        
        public void Run()
        {
            OnPlayerLoopEvent?.Invoke(timing);
        }
    }
}
