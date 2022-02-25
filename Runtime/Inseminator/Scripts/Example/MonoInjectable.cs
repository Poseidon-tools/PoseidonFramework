namespace Inseminator.Scripts.Example
{
    using Core.ViewManager;
    using Factory;
    using InseminatorExamples.DynamicContext;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class MonoInjectable : MonoBehaviour
    {
        [field: SerializeField, InseminatorAttributes.Inseminate, PreviewField]
        public MessageData SceneScopeMessageData { get; private set; }
        
        [SerializeField] private DynamicContextPrefab dynamicContextPrefab;
        #region Private Variables
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;
        [InseminatorAttributes.Inseminate] private InseminatorMonoFactory monoFactory;
        #endregion
        #region Unity Methods
        private void Start()
        {
            Debug.Log($"Hey, it's ${name} here! I'm using properly injected ViewManager: {viewManager.name} and property injection into message: {SceneScopeMessageData.Message}");
            
            Invoke(nameof(InstantiateDynamicContextPrefab), 10f);
        }
        #endregion

        [Button]
        private void InstantiateDynamicContextPrefab()
        {
            monoFactory.Create<DynamicContextPrefab>(dynamicContextPrefab);
        }
    }
}