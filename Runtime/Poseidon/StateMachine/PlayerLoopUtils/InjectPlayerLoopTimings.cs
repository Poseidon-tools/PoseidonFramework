namespace Poseidon.StateMachine.PlayerLoopUtils
{
    using System;

    [Flags]
    public enum InjectPlayerLoopTimings
    {
        /// <summary>
        /// Preset: All loops(default).
        /// </summary>
        All =
            Initialization | LastInitialization |
            EarlyUpdate | LastEarlyUpdate |
            FixedUpdate | LastFixedUpdate |
            PreUpdate | LastPreUpdate |
            Update | LastUpdate |
            PreLateUpdate | LastPreLateUpdate |
            PostLateUpdate | LastPostLateUpdate
#if UNITY_2020_2_OR_NEWER
            | TimeUpdate | LastTimeUpdate,
#else
            ,
#endif

        /// <summary>
        /// Preset: All without last except LastPostLateUpdate.
        /// </summary>
        Standard =
            Initialization |
            EarlyUpdate |
            FixedUpdate |
            PreUpdate |
            Update |
            PreLateUpdate |
            PostLateUpdate | LastPostLateUpdate
#if UNITY_2020_2_OR_NEWER
            | TimeUpdate
#endif
        ,

        /// <summary>
        /// Preset: Minimum pattern, Update | FixedUpdate | LastPostLateUpdate
        /// </summary>
        Minimum =
            Update | FixedUpdate | LastPostLateUpdate,

        // PlayerLoopTiming

        Initialization = 1,
        LastInitialization = 2,

        EarlyUpdate = 4,
        LastEarlyUpdate = 8,

        FixedUpdate = 16,
        LastFixedUpdate = 32,

        PreUpdate = 64,
        LastPreUpdate = 128,

        Update = 256,
        LastUpdate = 512,

        PreLateUpdate = 1024,
        LastPreLateUpdate = 2048,

        PostLateUpdate = 4096,
        LastPostLateUpdate = 8192

#if UNITY_2020_2_OR_NEWER
        ,
        // Unity 2020.2 added TimeUpdate https://docs.unity3d.com/2020.2/Documentation/ScriptReference/PlayerLoop.TimeUpdate.html
        TimeUpdate = 16384,
        LastTimeUpdate = 32768
#endif
    }
}