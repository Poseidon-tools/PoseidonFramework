namespace Inseminator.Scripts.Example
{
    using Core.ViewManager;
    using UnityEngine;

    public class TestNestedModuleInjection 
    {
        #region Private Variables
        [InseminatorAttributes.Inseminate] private ViewManager usedViewManager;
        [InseminatorAttributes.Surrogate] private NestedInNested nested = new NestedInNested();
        [InseminatorAttributes.Surrogate(ForceInitialization = true)] private NestedInNested nestedUninitialized;
        #endregion
        #region Public API
        public void Alert()
        {
            Debug.Log($"Used ViewManager is: {usedViewManager.name}", usedViewManager);
            nested.Alert();
            // variable was not initialized manually using ctor
            nestedUninitialized.Alert();
        }
        #endregion
    }


    public class NestedInNested
    {
        #region Private Variables
        [InseminatorAttributes.Inseminate] private MessageData messageData;
        [InseminatorAttributes.Surrogate] private Nested3rdLevelTest deeperLevelTest = new Nested3rdLevelTest();
        #endregion
        #region Public API
        
        public void Alert()
        {
            Debug.Log($"Message: {messageData.Message}");
            deeperLevelTest.Alert();
        }
        #endregion
    }

    public struct Nested3rdLevelTest
    {
        #region Private Variables
        [InseminatorAttributes.Inseminate] private MessageData messageData;
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;
        #endregion
        #region Public API
        
        public void Alert()
        {
            Debug.Log($"Struct: Message: {messageData.Message}");
            Debug.Log($"Struct: ViewManager: {viewManager.name}");
        }
        #endregion
    }
}