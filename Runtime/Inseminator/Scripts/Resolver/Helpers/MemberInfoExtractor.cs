namespace Inseminator.Scripts.Resolver.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MemberInfoExtractor
    {
        #region Public API
        public List<MemberInfo> GetMembers(MemberTypes memberType, object instance, BindingFlags bindingFlags)
        {
            if (instance == null) return new List<MemberInfo>();
            switch (memberType)
            {
                case MemberTypes.Property:
                    return instance.GetType().GetProperties(bindingFlags).Select(p => (MemberInfo)p).ToList();
                case MemberTypes.Field:
                    return instance.GetType().GetFields(bindingFlags).Select(p => (MemberInfo)p).ToList();
            }

            return new List<MemberInfo>();;
        }

        public MemberInfo GetMember(MemberTypes memberType, string name, object sourceObject, BindingFlags bindingFlags)
        {
            switch (memberType)
            {
                case MemberTypes.Field:
                    return sourceObject.GetType().GetField(name, bindingFlags);
                case MemberTypes.Property:
                    return sourceObject.GetType().GetProperty(name, bindingFlags);
                default:
                    return null;
            }
        }
        #endregion

        
    }
}