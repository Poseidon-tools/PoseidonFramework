namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Data.Baking;
    using Helpers;
    using ReflectionBaking;
    using UnityEngine;

    public class PrebakedInjectionModule : ResolvingModule
    {
        #region Private Variables
        private ReflectionBakingData bakingData;
        private InseminatorDependencyResolver dependencyResolver;
        private MemberInfoExtractor memberInfoExtractor = new MemberInfoExtractor();
        #endregion
        #region Public API
        public override void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject)
        {
            this.dependencyResolver = dependencyResolver;
            
            bakingData = ReflectionBaker.Instance.BakingData;
            BakedDataLookup(sourceObject);
        }
        #endregion

        #region Private Methods
        private void BakedDataLookup(object sourceObject)
        {
            if (sourceObject == null)
            {
                return;
            }
            if (!bakingData.BakedInjectableFields.TryGetValue(sourceObject.GetType(), out var fieldBakingDatas)) return;
            foreach (var fieldBakingData in fieldBakingDatas)
            {
                var memberInfo = memberInfoExtractor.GetMember(fieldBakingData.MemberType, fieldBakingData.MemberName,
                    sourceObject, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
                if (memberInfo == null)
                {
                    Debug.LogError($"Failed to get member: {fieldBakingData.MemberName}");
                    continue;
                }
                var instance = ResolveSingleDependency(memberInfo.GetUnderlyingType(), fieldBakingData.Attribute?.InstanceId);
                memberInfo.SetValue(sourceObject, instance);
            }
            ResolveAndRunBakedMethods(sourceObject);

            ResolveNested(ref sourceObject);
        }
        
        private void ResolveNested(ref object parentInstance)
        {
            if (parentInstance == null)
            {
                return;
            }
            if (!bakingData.BakedSurrogateFields.TryGetValue(parentInstance.GetType(),
                out var surrogateFieldBakingDatas))
            {
                return;
            }

            foreach (var surrogateFieldBakingData in surrogateFieldBakingDatas)
            {
                var memberInfo = memberInfoExtractor.GetMember(surrogateFieldBakingData.MemberType,
                    surrogateFieldBakingData.MemberName, parentInstance,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (memberInfo == null)
                {
                    Debug.LogError($"Failed to get member: {surrogateFieldBakingData.MemberName}");
                    continue;
                }
                var nestedInstance = memberInfo.GetValue(parentInstance);
                if (surrogateFieldBakingData.Attribute.ForceInitialization)
                {
                    nestedInstance = Activator.CreateInstance(memberInfo.GetUnderlyingType());
                    if(nestedInstance == null)
                    {
                        Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    memberInfo.SetValue(parentInstance, nestedInstance);
                }
                    
                BakedDataLookup(nestedInstance);
                if (memberInfo.GetUnderlyingType().IsValueType)
                {
                    memberInfo.SetValue(parentInstance, nestedInstance);
                }
            }
        }
        
        private object ResolveSingleDependency(Type targetType, object instanceId = null)
        {
            if (!dependencyResolver.RegisteredDependencies.TryGetValue(targetType, out var dependency))
            {
                Debug.LogError($"Cannot get dependency instance for {targetType.Name} | {targetType}");
                return default;
            }
            if (instanceId == null)
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }

        private void ResolveAndRunBakedMethods(object sourceObject)
        {
            var targetType = sourceObject.GetType();
            List<object> resolvedParameters = new List<object>();
            if (!bakingData.BakedMethods.TryGetValue(targetType, 
                out var bakedMethods)) return;
            foreach (var bakedMethod in bakedMethods)
            {
                resolvedParameters.Clear();
                int paramIndex = 0;
                foreach (var paramValue in bakedMethod.ParameterTypes.Select(parameterType 
                    => ResolveSingleDependency(parameterType, bakedMethod.Attribute.ParamIds[paramIndex])))
                {
                    if (paramValue == null)
                    {
                        paramIndex++;
                        continue;
                    }
                    resolvedParameters.Add(paramValue);
                    paramIndex++;
                }
                MethodsHelper.RunMethod(sourceObject, bakedMethod.MemberName, resolvedParameters);
            }
        }
        #endregion
    }
}