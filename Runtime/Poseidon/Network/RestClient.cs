namespace Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class RestClient
    {
        private enum Method
        {
            POST,
            PATCH,
            GET,
        }
        private const string AUTHORIZATION_KEY = "AuthorizationKey";
        
        private static Dictionary<string, string> defaultRequestHeaders;
        private static Dictionary<string, string> DefaultRequestHeaders
        {
            get
            {
                // Sets default headers
                defaultRequestHeaders ??= new Dictionary<string, string>()
                {
                    {"Content-Type", "application/json"},
                    {"Accept", "application/json"},
                    {"X-App-Platform", "android"},
                    {"X-App-Version", Application.version},
                };
                if(PlayerPrefs.HasKey(AUTHORIZATION_KEY))
                {
                    string authorizationHeaderValue = $"Bearer {PlayerPrefs.GetString(AUTHORIZATION_KEY)}";
                    
                    if(!defaultRequestHeaders.ContainsKey("Authorization"))
                    {
                        defaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
                    }
                    defaultRequestHeaders["Authorization"] = authorizationHeaderValue;
                }
                return defaultRequestHeaders;
            }
        }

        /// <summary>
        /// Sets the Authorization value. It will be added to every request. It will also store it in PlayerPrefs.
        /// </summary>
        /// <param name="token">token</param>
        public static void SetAuthorization(string token)
        {
            Debug.Log($"setting new authorization token: {token}");
            string authorizationHeaderValue = $"Bearer {token}";
            
            if(!DefaultRequestHeaders.ContainsKey("Authorization"))
            {
                DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue );
            }
            defaultRequestHeaders["Authorization"] = authorizationHeaderValue;
            
            PlayerPrefs.SetString(AUTHORIZATION_KEY, token);
        }

        public static string GetAuthorizationToken()
        {
            return PlayerPrefs.HasKey(AUTHORIZATION_KEY) ? PlayerPrefs.GetString(AUTHORIZATION_KEY) : "";
        }

        /// <summary>
        /// Clears the Authorization header value and removes it from player prefs.
        /// </summary>
        public static void ClearAuthorization()
        {
            Debug.Log("clearing new authorization token");
            PlayerPrefs.DeleteKey(AUTHORIZATION_KEY);
            DefaultRequestHeaders.Remove("Authorization");
        }

        public static async UniTask<T> Get<T>(Uri uri, Action<float> progressCallBack = null) where T : struct
        {
            using var request = CreateRequest(uri, Method.GET);
            var progress = progressCallBack != null ? Progress.Create(progressCallBack) : null;

            try
            {
                await request.SendWebRequest().ToUniTask(progress);
                return JsonUtility.FromJson<T>(request.downloadHandler.text);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        public static async UniTask<T> Post<T>(Uri uri, string jsonData = null) where T : struct
        {
            using var request = CreateRequest(uri, Method.POST, jsonData);

            try
            {
                await request.SendWebRequest();
                return JsonUtility.FromJson<T>(request.downloadHandler.text);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }
        
        public static async UniTask<bool> Post(Uri uri, string jsonData)
        {
            using var request = CreateRequest(uri, Method.POST, jsonData);
            
            try
            {
                await request.SendWebRequest();
                return request.result == UnityWebRequest.Result.Success;
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        public static async UniTask<T> Patch<T>(Uri uri, string jsonData) where T : struct
        {
            using var request = CreateRequest(uri, Method.PATCH, jsonData);
            
            try
            {
                await request.SendWebRequest();
                return JsonUtility.FromJson<T>(request.downloadHandler.text);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        private static UnityWebRequest CreateRequest(Uri uri, Method method, string jsonData = null)
        {
            var request = new UnityWebRequest(uri, method.ToString());
            request.AddHeaderValues(DefaultRequestHeaders);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            if(string.IsNullOrEmpty(jsonData)) return request;

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);

            return request;
        }  
    }
}
