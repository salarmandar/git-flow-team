using Bgt.Ocean.Infrastructure.CustomAttributes;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Bgt.Ocean.Service.CustomJsonContractResolver
{
    public class IgnoreResolver : IContractResolver
    {
        public void ResolvePropertyMember(MemberInfo member, JsonProperty jsonProp)
        {
            if(member.HasCustomAttributeOfType<IgnoreJsonSerializeAttribute>())
            {
                jsonProp.Ignored = true;
            }
        }
    }
}
