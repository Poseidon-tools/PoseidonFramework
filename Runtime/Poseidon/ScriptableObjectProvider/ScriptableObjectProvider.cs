namespace Core.ScriptableObjectProvider
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    /// <summary>
    /// Generic scriptable object provider.
    /// Specify your target SO type when inheriting from it.
    /// Provides one single method to retrieve object from internal object list by predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScriptableObjectProvider<T> : ObjectProvider<T> where T: ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Just find all scriptable objects of given type in assets and fill list with results.
        /// </summary>
        [Button]
        protected void FindAllWithType()
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }

            providedObjects = assets;
        }
#endif
    }
}