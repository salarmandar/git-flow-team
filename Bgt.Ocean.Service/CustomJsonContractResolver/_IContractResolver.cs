using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace Bgt.Ocean.Service.CustomJsonContractResolver
{
    public interface IContractResolver
    {
        void ResolvePropertyMember(MemberInfo member, JsonProperty jsonProp);
    }

    public static class ContractResolverHelper
    {
        public static bool HasCustomAttributeOfType<T>(this MemberInfo member)
            where T : Attribute
            =>
            member.CustomAttributes.Any(e => e.AttributeType == typeof(T));
    }
}
