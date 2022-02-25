namespace Poseidon.Postman
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using UnityEngine;

    public static class Postman
    {
        #region Private Variables
        private static Dictionary<Type, List<Action<object>>> receivers = new Dictionary<Type, List<Action<object>>>();
        #endregion
        #region Public Methods
        public static void RegisterReceiver(IMessageReceiver receiver)
        {
            Debug.Log("received!");
            foreach (var listenedType in receiver.ListenedTypes)
            {
                if (receivers.TryGetValue(listenedType, out var existingItem))
                {
                    existingItem.Add(receiver.OnMessageReceived);
                    continue;
                }
                receivers.Add(listenedType, new List<Action<object>>(){receiver.OnMessageReceived});
            }
        }
        public static void UnregisterReceiver(IMessageReceiver receiver)
        {
            List<Type> entriesToRemove = new List<Type>();
            foreach (var listenedType in receiver.ListenedTypes)
            {
                if (receivers.TryGetValue(listenedType, out var existingItem))
                {
                    existingItem.Remove(receiver.OnMessageReceived);
                    if (existingItem.Count == 0)
                    {
                        entriesToRemove.Add(listenedType);
                    }
                }
            }
            // remove empty dictionary entries
            foreach (var entryToRemove in entriesToRemove)
            {
                receivers.Remove(entryToRemove);
            }
        }

        public static void Send<T>(T message)
        {
            if (!receivers.TryGetValue(typeof(T), out var receiversList)) return;
            for (var index = 0; index < receiversList.Count; index++)
            {
                var receiver = receiversList[index];
                receiver.Invoke(message);
            }
        }
        
        #endregion
    }
}