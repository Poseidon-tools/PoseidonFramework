using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils
{
    public static class FilesHelper
    { 
        public static string SaveToJson(object data, string filePath)
        {
            string json = JsonConvert.SerializeObject(data);
            string fullPath = Path.Combine(Application.persistentDataPath, filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, json);
            return fullPath;
        }

        public static bool TryLoadFromJson<T>(string path, out T dataObject)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Cannot load file: file doesn't exist at path: {path}");
                dataObject = default;
                return false;
            }

            string jsonData = File.ReadAllText(path);
            try
            {
                dataObject = JsonConvert.DeserializeObject<T>(jsonData);
                return true;
            }
            catch
            {
                dataObject = default;
                return false;
            }
        }

        public static byte[] ReadBytesFromFile(string path)
        {
            return File.Exists(path) ? File.ReadAllBytes(path) : new byte[0];
        }

        public static bool LoadImage(Texture2D texture2D, string imagePath)
        {
            byte[] imageData = ReadBytesFromFile(imagePath);
            return imageData.Length != 0 && texture2D.LoadImage(imageData);
        }

        public static string SaveTextureDataToPNG(string directory, string name, byte[] pngData)
        {
            return SaveFile(directory, name, pngData);
        }

        public static string SaveFile(string directory, string name, byte[] data)
        {
            string fullPath = Path.Combine(directory, name);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(fullPath, data);
            return fullPath;
        }
    }
}
