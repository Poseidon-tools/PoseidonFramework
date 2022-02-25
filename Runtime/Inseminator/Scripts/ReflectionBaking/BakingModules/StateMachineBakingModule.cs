namespace Inseminator.Scripts.ReflectionBaking.BakingModules
{
    using System.Collections.Generic;
    using System.Reflection;
    using Data.Baking;
    using Poseidon.StateMachine;
    using UnityEngine;

    public class StateMachineBakingModule : InseminatorBakingModule
    {
        private ReflectionBaker reflectionBaker;
        public override void Run(ref object sourceObject, ReflectionBakingData bakingData, ReflectionBaker reflectionBaker)
        {
            this.reflectionBaker = reflectionBaker;
            ResolveStateMachines(sourceObject, bakingData);
        }
        #region State Machine Helpers
        private void GetStates(object stateManagerInstance, ReflectionBakingData bakingData)
        {
            var properties = stateManagerInstance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.FieldType.IsArray)
                {
                    continue;
                }
                // check if array is the states array
                var elementType = propertyInfo.FieldType.GetElementType();
                if (elementType == null || !elementType.IsGenericType)
                {
                    //Debug.LogError($"{elementType?.Name} is not an generic type.");
                    continue;
                }
                if (elementType.GetGenericTypeDefinition() != typeof(State<>))
                {
                    //Debug.LogError($"{elementType.Name} is not an State<>, it's {elementType.GetGenericTypeDefinition()}");
                    continue;
                }
                //Debug.Log("Found states array!");
                var statesArray = propertyInfo.GetValue(stateManagerInstance) as object[];
                ResolveStates(statesArray, bakingData);
            }
        }

        private void ResolveStates(object[] statesArray, ReflectionBakingData bakingData)
        {
            if (statesArray.Length == 0)
            {
                Debug.LogError("passed uninitialized array!");
                return;
            }
            foreach (var state in statesArray)
            {
                var stateInstance = state;
                reflectionBaker.BakeSingle(ref stateInstance);
            }
        }

        #region Experimental
        public void ResolveStateMachines(object sourceObject, ReflectionBakingData bakingData)
        {
            var fields = sourceObject.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var targetType = fieldInfo.FieldType;
                if (!targetType.IsGenericType)
                {
                    //Debug.LogError($"Can't bake {fieldInfo.Name}: not a generic");
                    continue;
                }
               
                if (targetType.GetGenericTypeDefinition() != typeof(StateMachine<>))
                {
                    //Debug.LogError($"Can't bake {fieldInfo.Name}: not a StateMachine<>, its {targetType.GetGenericTypeDefinition()} ");
                    continue;
                }
                // this is StateManager<>
                var stateManagerInstance = fieldInfo.GetValue(sourceObject);
                if (bakingData.StateMachinesBaked.TryGetValue(sourceObject.GetType(), out var fieldNames))
                {
                    fieldNames.Add(fieldInfo.Name);
                }
                else
                {
                    bakingData.StateMachinesBaked.Add(sourceObject.GetType(), new List<string>(){fieldInfo.Name});
                }

                // get states from StateManager instance
                GetStates(stateManagerInstance, bakingData);
            }
        }
        #endregion
        
        #endregion
    }
}