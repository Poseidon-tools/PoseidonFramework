namespace Inseminator.Scripts.ReflectionBaking
{
#if UNITY_EDITOR
    using UnityEditor;

    public static class BakingMenuItem
    {
        [MenuItem("Tools/Inseminator/Bake")]
        public static void Bake()
        {
            ReflectionBaker.Instance.BakeAll();
        }
    }
#endif
    
}