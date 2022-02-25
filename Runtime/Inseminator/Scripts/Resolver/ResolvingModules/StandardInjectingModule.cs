namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Data;
    using Helpers;
    using UnityEngine;

    public sealed class StandardInjectingModule : ResolvingModule
    {
        private Dictionary<Type, List<InstallerEntity>> registeredDependencies;
        private InseminatorAttributes.Inseminate inseminateAttrCached;
        private InseminatorAttributes.Surrogate surrogateAttrCached;

        private MemberInfoExtractor memberInfoExtractor = new MemberInfoExtractor();
        private InseminatorDependencyResolver dependencyResolver;
        #region Public API
        public override void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject)
        {
            this.dependencyResolver = dependencyResolver;
            registeredDependencies = dependencyResolver.RegisteredDependencies;
            ResolveDependencies(ref sourceObject);
        }
        #endregion
        
        private void ResolveNested(ref object parentInstance)
        {
            var members = memberInfoExtractor.GetMembers(MemberTypes.Field, parentInstance,
                BindingFlags.Instance | BindingFlags.NonPublic);
            members.AddRange(memberInfoExtractor.GetMembers(MemberTypes.Property, parentInstance, BindingFlags.Instance | BindingFlags.Public));
            foreach (var fieldInfo in members)
            {
                if (!fieldInfo.IsDefined(typeof(InseminatorAttributes.Surrogate)))
                {
                    continue;
                }
                surrogateAttrCached = fieldInfo.GetCustomAttribute<InseminatorAttributes.Surrogate>(false);
                if (surrogateAttrCached == null)
                {
                    continue;
                }
                var nestedInstance = fieldInfo.GetValue(parentInstance);
                if (surrogateAttrCached.ForceInitialization)
                {
                    nestedInstance = TryForceInitializeInstance(fieldInfo.GetUnderlyingType());
                    if(nestedInstance == null)
                    {
                        Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
                    
                ResolveDependencies(ref nestedInstance);
                if (fieldInfo.GetUnderlyingType().IsValueType)
                {
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
            }
        }
        private void ResolveDependencies(ref object instanceObject)
        {
            List<MemberInfo> allInjectableMembers = new List<MemberInfo>();
            allInjectableMembers.AddRange(memberInfoExtractor.GetMembers(MemberTypes.Field, instanceObject, BindingFlags.Instance | BindingFlags.NonPublic));
            allInjectableMembers.AddRange(memberInfoExtractor.GetMembers(MemberTypes.Property, instanceObject, BindingFlags.Instance | BindingFlags.Public));
            
            foreach (var fieldInfo in allInjectableMembers)
            {
                if (!fieldInfo.IsDefined(typeof(InseminatorAttributes.Inseminate)))
                {
                    continue;
                }
                inseminateAttrCached = fieldInfo.GetCustomAttribute<InseminatorAttributes.Inseminate>( false);

                var instance = ResolveSingleDependency(fieldInfo.GetUnderlyingType(), inseminateAttrCached.InstanceId);
                fieldInfo.SetValue(instanceObject, instance);
            }
            
            MethodsHelper.ResolveMethods(instanceObject, dependencyResolver);
            
            ResolveNested(ref instanceObject);
        }

        private object ResolveSingleDependency(Type targetType, object instanceId = null)
        {
            if (!registeredDependencies.TryGetValue(targetType, out var dependency))
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
        
        #region Helpers
        private object TryForceInitializeInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
        #endregion
    }
}