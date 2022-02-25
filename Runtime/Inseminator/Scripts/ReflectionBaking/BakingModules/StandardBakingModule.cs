namespace Inseminator.Scripts.ReflectionBaking.BakingModules
{
    using System;
    using System.Reflection;
    using Data.Baking;
    using Resolver.Helpers;
    using UnityEngine;

    public class StandardBakingModule  : InseminatorBakingModule
    {
        public override void Run(ref object sourceObject, ReflectionBakingData bakingData, ReflectionBaker reflectionBaker)
        {
            GetMembersForBaking(ref sourceObject, bakingData);
        }
        
        private void GetMembersForBaking(ref object instanceObject, ReflectionBakingData bakingData)
        {
            var allInjectableMembers = memberInfoExtractor.GetMembers(MemberTypes.Field, instanceObject,
                BindingFlags.Instance | BindingFlags.NonPublic);
            allInjectableMembers.AddRange(memberInfoExtractor.GetMembers(MemberTypes.Property, instanceObject, BindingFlags.Instance | BindingFlags.Public));
            
            foreach (var memberInfo in allInjectableMembers)
            {
                if (!memberInfo.IsDefined(typeof(InseminatorAttributes.Inseminate)))
                {
                    continue;
                }
                var inseminateAttr = memberInfo.GetCustomAttribute<InseminatorAttributes.Inseminate>( false);
                ReflectionBaker.UpdateBakingDataWithField(bakingData, instanceObject, memberInfo, inseminateAttr);
            }
            BakeMethods(instanceObject, bakingData);
            HandleSurrogates(ref instanceObject, bakingData);
        }

        private void BakeMethods(object sourceObject, ReflectionBakingData bakingData)
        {
            foreach (var bakedMethodTuple in MethodsHelper.GetInseminationMethods(sourceObject))
            {
                ReflectionBaker.UpdateBakingDataWithMethod(bakingData, sourceObject, bakedMethodTuple.Item1, bakedMethodTuple.Item2);
            }
        }
        
        private void HandleSurrogates(ref object parentInstance, ReflectionBakingData bakingData)
        {
            var allMembers = memberInfoExtractor.GetMembers(MemberTypes.Field, parentInstance,
                BindingFlags.Instance | BindingFlags.NonPublic);
            allMembers.AddRange(memberInfoExtractor.GetMembers(MemberTypes.Property, parentInstance, BindingFlags.Instance | BindingFlags.Public));
            foreach (var memberInfo in allMembers)
            {
                if (!memberInfo.IsDefined(typeof(InseminatorAttributes.Surrogate)))
                {
                    continue;
                }
                var surrogateAttr = memberInfo.GetCustomAttribute<InseminatorAttributes.Surrogate>(false);
                if (surrogateAttr == null)
                {
                    continue;
                }

                ReflectionBaker.UpdateBakingDataWithSurrogate(bakingData, parentInstance, memberInfo, surrogateAttr);
                
                var nestedInstance = memberInfo.GetValue(parentInstance);
                if (surrogateAttr.ForceInitialization)
                {
                    nestedInstance = Activator.CreateInstance(memberInfo.GetUnderlyingType());
                    if(nestedInstance == null)
                    {
                        Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    memberInfo.SetValue(parentInstance, nestedInstance);
                }
                
                GetMembersForBaking(ref nestedInstance, bakingData);
            }
        }
    }
}