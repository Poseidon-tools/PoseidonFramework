namespace Poseidon.StateMachine.Editor
{
    using UnityEditor;
    using System.Reflection;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public static class StatesGenerator
    {
        private const string NAMESPACE_PATTERN = @"\b(?<=namespace )([^\n\r ]*)\b";
        private const string ENUM_PATTERN = @"\b(?<=enum )([^\n\r ]*)\b";
        
        [MenuItem("Assets/Poseidon/Generate States")] 
        private static void GenerateState()
        {
            foreach (var obj in Selection.objects)
            {
                string enumName = "";
                string namespaceName = "";
                
                if (obj is MonoScript monoScript)
                {
                    string scriptText = monoScript.text;

                    namespaceName = FindMatch(scriptText, NAMESPACE_PATTERN);
                    enumName = FindMatch(scriptText, ENUM_PATTERN);

                    Debug.Log(namespaceName);
                    Debug.Log(enumName);
                }

                Type enumType = GetEnumType(namespaceName, enumName);
                var enumValues = GetEnumValues(enumType);

                if (enumValues == null)
                {
                    Debug.LogError("NO ENUM VALUES WERE FOUND");
                    return;
                }
                
                // Open folder panel 
                string directory = EditorUtility.OpenFolderPanel("Choose location for States", Application.dataPath, "");

                // Canceled choose? Do nothing.
                if (string.IsNullOrEmpty(directory)) return;
                
                // create namespace based on folder structure
                int scriptsIndex = directory.IndexOf("Scripts/", StringComparison.Ordinal);
                int assetsIndex = directory.IndexOf("Assets/", StringComparison.Ordinal);

                string namespaceInFile = scriptsIndex < 0 
                    ? directory.Substring(assetsIndex + "Assets/".Length).Replace("/", ".")
                    : directory.Substring(scriptsIndex + "Scripts/".Length).Replace("/", ".")
                    ;

                foreach (string enumValue in enumValues)
                {
                    string className = $"{enumValue}State";
                    string classNameInFile = $"{className} : State<{enumName}>";
                    string fileName = Path.Combine(directory, $"{className}.cs");

                    StateFileGenerator.GenerateStateFile(fileName, namespaceInFile, classNameInFile, enumName, enumValue, 
                        "Poseidon.StateMachine", enumType.Namespace);
                }
                
                // Refresh
                AssetDatabase.Refresh();
            }
        }

        private static Type GetEnumType(string namespaceName, string enumName)
        {
            // Search for the type in all assemblies. Unfortunately we don't know which assembly will contain the type 
            // since we can add assembly definition.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                // get enum type by namespace and enum name
                Type enumType = assembly.GetType($"{namespaceName}.{enumName}");
                
                // continue if not found
                if (enumType == null || !enumType.IsEnum) continue;

                return enumType;
            }

            return null;
        }
        
        private static IEnumerable<string> GetEnumValues(Type enumType)
        {
            // get enum values 
            Array values = Enum.GetValues(enumType);

            // feed the list with enum values
            var enumValues = new List<string>(values.Length);
            enumValues.AddRange(from object value in values select value.ToString());
            return enumValues;
        }

        private static string FindMatch(string input, string pattern)
        {
            // save the namespace if we have a match
            // note that we only want to grab the first result
            Match match = Regex.Match(input, pattern, RegexOptions.Compiled);
            return match.Success ? match.Groups[0].Value : string.Empty;
        }
    }
}
