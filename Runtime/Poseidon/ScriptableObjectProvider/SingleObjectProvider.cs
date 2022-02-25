namespace Core.ScriptableObjectProvider
{
    using System;
    using UnityEngine;

    public abstract class SingleObjectProvider<T> : ScriptableObject where T : class
    {
        [NonSerialized] private T providedObject;

        public void Register(T providedObject)
        {
            this.providedObject = providedObject;
            //Debug.Log($"<color=blue>{typeof(T)} registered</color>");
        }

        public bool IsRegistered() => providedObject != null;

        public T Get()
        {
            if (providedObject == null)
            {
                Debug.LogWarningFormat(
                                       "[ScriptableProvider] ScriptableProvider {0} is not registered. Try to register in Awake.",
                                       typeof(T));
            }

            ;
            //Debug.Log($"<color=yellow>{typeof(T)} Get</color>");
            return providedObject;
        }

        public void Reset()
        {
            //Debug.Log($"<color=green>{typeof(T)} reset</color>");
            providedObject = null;
        }
    }
}