namespace Utils.ScenesHelper
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.SceneManagement;

    public static class ScenesHelper
    {
        public static List<string> GetAvailableScenesRawList()
        {
            return GetScenesInBuild().ToList();
        }


        private static string[] GetScenesInBuild()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;     
            string[] scenes = new string[sceneCount];
            for(int i = 0; i < sceneCount; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }

            return scenes;
        }
    }
}