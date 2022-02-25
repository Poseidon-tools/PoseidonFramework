namespace Core.ScriptableObjectProvider
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class ObjectProvider<T> : SerializedScriptableObject,  IObjectProvider<T>
    {
        #region Inspector
        [BoxGroup("Provided objects")] [SerializeField]
        protected bool useCustomProviding;
        [BoxGroup("Provided objects")] [SerializeField] [HideIf(nameof(useCustomProviding))]
        [InfoBox("Specify all objects of your type, that you want to provide.")]
        protected List<T> providedObjects;
        #endregion
        #region Public Methods
        /// <summary>
        /// Get object matching condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual T GetObject(Predicate<T> condition)
        {
            return providedObjects != null ? providedObjects.Find(condition) : default(T);
        }
        public virtual List<T> GetObjects(Predicate<T> condition)
        {
            return providedObjects;
        }
        #endregion
    }
}