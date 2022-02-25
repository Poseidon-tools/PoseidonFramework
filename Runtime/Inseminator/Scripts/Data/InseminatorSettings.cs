namespace Inseminator.Scripts.Data
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Inseminator/Settings", fileName = "InseminatorSettings")]
    public sealed class InseminatorSettings : ScriptableObject  
    {
        #region Inspector
        [field: SerializeField] public bool AutoBakeOnBuild { get; private set; }
        [field: SerializeField, Multiline(5)] public List<string> BakeablePrefabsPaths { get; private set; }
        #endregion
    }
}