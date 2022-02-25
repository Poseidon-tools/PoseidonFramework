namespace Inseminator.Scripts.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Data/MessageData")]
    public class MessageData : ScriptableObject
    {
        [field: SerializeField, Header("Message"), Multiline(5)]
        public string Message { get; private set; }
    }
}