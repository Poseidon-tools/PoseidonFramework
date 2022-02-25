namespace Utils.ScenesHelper
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface ISceneMaintainer
    {
        UniTask LoadScene(string sceneName, Action<List<GameObject>> onLoadedCallback = null);
        UniTask UnloadScene(string sceneName, Action onUnloadCallback = null);
        T GetComponentInRootObjects<T>() where T : Component;
    }
}