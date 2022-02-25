namespace Inseminator.Scripts.Resolver.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Unity.VisualScripting;
    using UnityEngine;

    public static class MethodsHelper
    {
        #region Private Variables
        private static InseminatorDependencyResolver dependencyResolver;
        #endregion
        #region Public API
        public static void ResolveMethods(object sourceObject, InseminatorDependencyResolver resolver)
        {
            dependencyResolver = resolver;
            ResolveAndRun(FilterMethodsByAttribute(GetMethods(sourceObject)), sourceObject);
        }

        public static void RunMethod(object methodOwner, string methodName, List<object> methodParameters)
        {
            var methodInfo = methodOwner.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (methodInfo == null)
            {
                Debug.LogError($"No such method {methodName} inside {methodOwner.GetType().Name} object.");
                return;
            }

            if (methodInfo.GetParameters().Length != methodParameters.Count)
            {
                Debug.LogError($"Expected {methodInfo.GetParameters().Length} params, received {methodParameters.Count}");
                return;
            }

            methodInfo.Invoke(methodOwner, methodParameters.ToArray());
        }

        public static List<(MethodInfo, InseminatorAttributes.InseminateMethod)> GetInseminationMethods(object sourceObject)
        {
            var filteredMethods = FilterMethodsByAttribute(GetMethods(sourceObject));
            var resultList = new List<(MethodInfo, InseminatorAttributes.InseminateMethod)>();
            foreach (var filteredMethod in filteredMethods)
            {
                resultList.Add((filteredMethod, filteredMethod.GetAttribute<InseminatorAttributes.InseminateMethod>()));
            }

            return resultList;
        }

        #endregion

        #region Private Methods
        private static MethodInfo[] GetMethods(object sourceObject)
        {
            return sourceObject.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static List<MethodInfo> FilterMethodsByAttribute(MethodInfo[] methodInfos)
        {
            return methodInfos.Where(methodInfo => methodInfo.IsDefined(typeof(InseminatorAttributes.InseminateMethod))).ToList();
        }

        private static void ResolveAndRun(List<MethodInfo> filteredMethods, object methodOwner)
        {
            foreach (var methodInfo in filteredMethods)
            {
                var attr = methodInfo.GetCustomAttribute<InseminatorAttributes.InseminateMethod>();
                var paramCount = methodInfo.GetParameters().Length;
                var paramValues = ResolveMethodParamDependencies(methodInfo, attr, dependencyResolver);
               
                if (paramCount != paramValues.Count)
                {
                    return;
                }
                methodInfo.Invoke(methodOwner, paramValues.ToArray());
            }
        }

        private static List<object> ResolveMethodParamDependencies(MethodInfo methodInfo, InseminatorAttributes.InseminateMethod attr, InseminatorDependencyResolver dependencyResolver)
        {
            var paramInjectingValues = new List<object>();
            if (attr == null)
            {
                return paramInjectingValues;
            }

            var methodParams = methodInfo.GetParameters();
            if (methodParams.Length == 0)
            {
                return paramInjectingValues;
            }
            
            int paramIndex = 0;
            foreach (var parameterInfo in methodParams)
            {
                var paramValue = dependencyResolver.GetValueForType(parameterInfo.ParameterType, attr.ParamIds[paramIndex]);
                if (paramValue == null)

                {
                    paramIndex++;
                    continue;
                }
                paramInjectingValues.Add(paramValue);
                paramIndex++;
            }

            return paramInjectingValues;
        }
        #endregion
    }
}