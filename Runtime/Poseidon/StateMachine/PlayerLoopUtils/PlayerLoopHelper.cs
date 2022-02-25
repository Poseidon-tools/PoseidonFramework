namespace Poseidon.StateMachine.PlayerLoopUtils
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.LowLevel;
    using PlayerLoopType = UnityEngine.PlayerLoop;

    public static class PlayerLoopHelper
    {
        private struct PoseidonLoopRunnerEarlyUpdate { };
        private struct PoseidonLoopRunnerFixedUpdate { };
        private struct PoseidonLoopRunnerPreUpdate { };
        private struct PoseidonLoopRunnerUpdate { };
        private struct PoseidonLoopRunnerPreLateUpdate { };
        private struct PoseidonLoopRunnerPostLateUpdate { };
        
            
        private static readonly (Type playerLoopType, Type loopRunnerType, PlayerLoopTiming playerLoopTiming)[] loopsTypes =
        {
            // EarlyUpdate
            (typeof(PlayerLoopType.EarlyUpdate), typeof(PoseidonLoopRunnerEarlyUpdate), PlayerLoopTiming.EarlyUpdate),
            // FixedUpdate
            (typeof(PlayerLoopType.FixedUpdate), typeof(PoseidonLoopRunnerFixedUpdate), PlayerLoopTiming.FixedUpdate),
            // PreUpdate
            (typeof(PlayerLoopType.PreUpdate), typeof(PoseidonLoopRunnerPreUpdate), PlayerLoopTiming.PreUpdate),
            // Update
            (typeof(PlayerLoopType.Update), typeof(PoseidonLoopRunnerUpdate), PlayerLoopTiming.Update),
            // PreLateUpdate
            (typeof(PlayerLoopType.PreLateUpdate), typeof(PoseidonLoopRunnerPreLateUpdate), PlayerLoopTiming.PreLateUpdate),
            // PostLateUpdate
            (typeof(PlayerLoopType.PostLateUpdate), typeof(PoseidonLoopRunnerPostLateUpdate), PlayerLoopTiming.PostLateUpdate),
        };

        private static PlayerLoopRunner[] runners;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (runners != null) return; // already initialized
            
            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Initialize(ref playerLoop);
        }
        
        private static void Initialize(ref PlayerLoopSystem playerLoop)
        {
            runners = new PlayerLoopRunner[loopsTypes.Length];

            var copyList = playerLoop.subSystemList.ToArray();

            for (int i = 0; i < loopsTypes.Length; i++)
            {
                (Type playerLoopType, Type loopRunnerType, PlayerLoopTiming playerLoopTiming) = loopsTypes[i];
                InsertLoop(copyList, playerLoopType, i, loopRunnerType, playerLoopTiming);
            }
            
            playerLoop.subSystemList = copyList;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }
        
        private static void InsertLoop(PlayerLoopSystem[] copyList, Type loopType, int index, Type loopRunnerType, PlayerLoopTiming playerLoopTiming)
        {
            int i = FindLoopSystemIndex(copyList, loopType);
            
            runners[index] = new PlayerLoopRunner(playerLoopTiming);
            copyList[i].subSystemList = InsertRunner(copyList[i], loopRunnerType, runners[index]);
        }
        
        private static PlayerLoopSystem[] InsertRunner(PlayerLoopSystem loopSystem, Type loopRunnerType, PlayerLoopRunner runner)
        {
            PlayerLoopSystem runnerLoop = new PlayerLoopSystem
            {
                type = loopRunnerType,
                updateDelegate = runner.Run
            };

            // Remove items from previous initializations.
            var source = RemoveRunner(loopSystem, loopRunnerType);
            var dest = new PlayerLoopSystem[source.Length + 1];

            Array.Copy(source, 0, dest, 1, source.Length);
            dest[0] = runnerLoop;

            return dest;
        }

        private static PlayerLoopSystem[] RemoveRunner(PlayerLoopSystem loopSystem, Type loopRunnerType)
        {
            return loopSystem.subSystemList
                .Where(ls => ls.type != loopRunnerType)
                .ToArray();
        }

        private static int FindLoopSystemIndex(PlayerLoopSystem[] playerLoopList, Type systemType)
        {
            for (int i = 0; i < playerLoopList.Length; i++)
            {
                if (playerLoopList[i].type == systemType)
                {
                    return i;
                }
            }

            throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
        }
    }
}