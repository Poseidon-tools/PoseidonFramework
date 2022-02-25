namespace Inseminator.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class InseminatorHelpers
    {
        #region Helper methods
      
        public static List<GameObject> GetSceneObjectsExceptTypes(List<Type> excludedTypes, Scene scene)
        {
            List<GameObject> rootObjects = new List<GameObject>();
            
            scene.GetRootGameObjects(rootObjects);
            List<GameObject> sceneObjectsFiltered = new List<GameObject>();
            foreach (var rootObject in rootObjects)
            {
                GetChildrenWithCondition(rootObject, sceneObjectsFiltered, targetObject =>
                {
                    return excludedTypes.All(excludedType => targetObject.GetComponent(excludedType) == null);
                });
            }

            return sceneObjectsFiltered;
        }
        
        public static List<GameObject> GetRootSceneObjects(Scene scene)
        {
            var rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            return rootObjects;
        }
        
        public static List<MonoBehaviour> GetAllComponents(List<GameObject> sourceObjectsList)
        {
            List<MonoBehaviour> components = new List<MonoBehaviour>();
            foreach (var listItem in sourceObjectsList)
            {
                components.AddRange(listItem.GetComponents<MonoBehaviour>());
            }
            return components;
        }
        
        public static List<MonoBehaviour> GetComponentsExceptTypes(List<GameObject> sourceObjectsList, List<Type> excludedTypes)
        {
            List<MonoBehaviour> components = new List<MonoBehaviour>();
            
            foreach (var listItem in sourceObjectsList)
            {
                components.AddRange(listItem.GetComponents<MonoBehaviour>().Where(c => !excludedTypes.Contains(c.GetType())).ToList());
            }
            return components;
        }

        public static object TryResolveInParentHierarchy<T>(InseminatorDependencyResolver resolver, 
            string objectId = "")
        {
            var result = resolver.GetValueForType(typeof(T), objectId);
            return result ?? TryResolveInParentHierarchy<T>( resolver.Parent, objectId);
        }
        #endregion

        #region Private Methods
        private static void GetChildrenWithCondition(GameObject parentObject, List<GameObject> outputList, Predicate<GameObject> condition)
        {
            if (condition.Invoke(parentObject))
            {
                outputList.Add(parentObject);
            }
            int childCount = parentObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GetChildrenWithCondition(parentObject.transform.GetChild(i).gameObject, outputList, condition);
            }
        }
        #endregion
    }
}