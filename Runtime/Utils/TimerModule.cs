namespace Utils
{
    using System;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;

    public class TimerModule
    {
        #region Private Variables
        private TimeSpan timeSpan;
        private Action<TimeSpan> onTick;
        private CoroutineHandle timerHandle;
        #endregion

        #region Public API
        public void Start(Action<TimeSpan> onTickCallback)
        {
            if (onTick != null)
            {
                Debug.Log("Couldn't start timer - already started.");
                return;
            }
            onTick = onTickCallback;

            timerHandle = Timing.RunCoroutine(TickCoroutine());
        }

        public void Pause()
        {
            Timing.PauseCoroutines(timerHandle);
        }

        public void Resume()
        {
            Timing.ResumeCoroutines(timerHandle);
        }

        public void Stop()
        {
            Timing.KillCoroutines(timerHandle);
            timeSpan = TimeSpan.Zero;
        }

        public void Dispose()
        {
            onTick = null;
            Stop();
        }
        #endregion
        #region Private Methods
        private IEnumerator<float> TickCoroutine()
        {
            while(true)
            {
                yield return Timing.WaitForSeconds(1f);
                timeSpan += TimeSpan.FromSeconds(1f);
                onTick?.Invoke(timeSpan);
            }
        }
        #endregion
    }
}