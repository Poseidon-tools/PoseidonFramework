namespace Inseminator.Scripts.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "DI/Example/ExampleSOInjection")]
    public class ExampleSOInjection : ScriptableObject
    {
        #region Inspector
        [SerializeField, InseminatorAttributes.Inseminate]
        private MessageData messageData;
        #endregion
    }
}