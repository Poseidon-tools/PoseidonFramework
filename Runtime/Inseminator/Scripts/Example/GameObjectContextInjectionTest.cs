namespace Inseminator.Scripts.Example
{
    using Core.ViewManager;
    using UnityEngine;

    public class GameObjectContextInjectionTest : MonoBehaviour
    {
        #region Private Variables
        [InseminatorAttributes.Inseminate] private MessageData messageData;
        [InseminatorAttributes.Inseminate] private ViewManager sceneViewManager;
        private MessageData secondaryMessage;
        #endregion
        #region Unity Methods
        private void Start()
        {
            Debug.Log($"Hey hey, it's {name} here! Injected message is: {messageData.Message}.");
            Debug.Log($"{name}: Scene View Manager: {sceneViewManager.name}", sceneViewManager);
        }
        #endregion
        #region Method Injection Test
        
        [InseminatorAttributes.InseminateMethod(ParamIds = new object[]
            {"SampleMessage", "SecondaryMessage"})]
        private void TestMethodInjection(MessageData messageData, MessageData secondaryMessage)
        {
            this.secondaryMessage = secondaryMessage;
            
            Debug.Log("Method injection messages:");
            Debug.Log($"Sample: {messageData.Message}");
            Debug.Log($"Secondary: {secondaryMessage.Message}");
        }
        #endregion
    }
}