namespace Utils.ScenesHelper
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneMaintainer : ISceneMaintainer
    {
        #region Private Variables
        private Scene currentLoadedScene;
        #endregion
        
        #region Public Methods
        public async UniTask LoadScene(string sceneName, Action<List<GameObject>> onLoadedCallback = null)
        {
            if(sceneName.IsNullOrEmpty())
            {
                Debug.LogError("Cannot load scene: invalid build index.");
                return;
            }

            // unload previous scene if current scene is valid
            if(currentLoadedScene.buildIndex>=0)
            {
                await SceneManager.UnloadSceneAsync(currentLoadedScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).ToUniTask();
            }

            // load new scene
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            // assign current scene from buildIndex
            int sceneBuildIndex = SceneManager.GetSceneByName(sceneName).buildIndex;
            currentLoadedScene = GetCurrentScene(sceneBuildIndex);

            // get and return root objects if callback isn't null
            onLoadedCallback?.Invoke(GetSceneRootObjects(sceneBuildIndex));
        }
        
        public async UniTask UnloadScene(string sceneName, Action onUnloadCallback = null)
        {
            await SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).ToUniTask();
            onUnloadCallback?.Invoke();
        }

        public T GetComponentInRootObjects<T>() where T : Component
        {
            var result = new List<GameObject>(currentLoadedScene.GetRootGameObjects());
            foreach(var gameObject in result)
            {
                gameObject.TryGetComponent(out T component);
                if(component != null)
                {
                    return component;
                }
            }
            return default;
        }

        
        #endregion

        #region Private Methods

        private static Scene GetCurrentScene(int buildIndex)
        {
            for(int i = 0; i < SceneManager.sceneCount; i++)
            {
                var currentScene = SceneManager.GetSceneAt(i);
                if(currentScene.buildIndex != buildIndex) continue;
                return currentScene;
            }
            return default;
        }
        
        private List<GameObject> GetSceneRootObjects(int buildIndex)
        {
            var result = new List<GameObject>();
            result.AddRange(currentLoadedScene.GetRootGameObjects());
            return result;
        }
        
        #endregion
    }
}