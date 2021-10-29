using System.Reflection;
using Newtonsoft.Json.Serialization;
using Bgt.Ocean.Infrastructure.CustomAttributes;

namespace Bgt.Ocean.Service.CustomJsonContractResolver
{
    public class NameResolver : IContractResolver
    {
        public void ResolvePropertyMember(MemberInfo member, JsonProperty jsonProp)
        {
            if(member.HasCustomAttributeOfType<ChangeJsonPropNameAttribute>())
            {
                jsonProp.PropertyName = member.GetCustomAttribute<ChangeJsonPropNameAttribute>().Name;
            }
        }
    }
}
