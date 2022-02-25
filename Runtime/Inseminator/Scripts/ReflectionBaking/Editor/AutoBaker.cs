namespace Inseminator.Scripts.ReflectionBaking
{
#if UNITY_EDITOR
    using Data;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using Utils;

    internal class AutoBaker : IPreprocessBuildWithReport 
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            var settings = AssetsUtils.FindFirstAsset<InseminatorSettings>();
            if(settings.AutoBakeOnBuild)
            {
                ReflectionBaker.Instance.BakeAll();
            }
        }
    }
#endif
}