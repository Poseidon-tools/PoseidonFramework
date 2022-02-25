namespace Core.Network
{
    using System.Collections.Generic;
    using UnityEngine.Networking;

    public static class UnityWebRequestExtensions
    {
        public static void AddHeaderValues(this UnityWebRequest request, Dictionary<string, string> headerValues)
        {
            foreach(var entity in headerValues)
            {
                request.SetRequestHeader(entity.Key, entity.Value);
            }
        }
    }
}
