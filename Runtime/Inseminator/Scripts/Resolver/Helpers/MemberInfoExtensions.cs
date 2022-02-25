namespace Inseminator.Scripts.Resolver.Helpers
{
    using System;
    using System.Reflection;
    using UnityEngine;

    public static class MemberInfoExtensions
    {
        #region Private Methods
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be of type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public static void SetValue(this MemberInfo memberInfo, object instance, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo) memberInfo).SetValue(instance, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo) memberInfo).SetValue(instance, value);
                    break;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be of type FieldInfo or PropertyInfo"
                    );
            }
        }
        public static object GetValue(this MemberInfo memberInfo, object instance)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).GetValue(instance);
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).GetValue(instance);
                default:
                    Debug.LogError($"Cannot get value for {memberInfo.Name} and {memberInfo.MemberType}");
                    return null;
            }
        }
        #endregion
    }
}