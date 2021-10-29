using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Bgt.Ocean.Service.CustomJsonContractResolver
{
    public class MainContractResolver : ContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProp = base.CreateProperty(member, memberSerialization);

            foreach (var item in _resolver)
            {
                item.ResolvePropertyMember(member, jsonProp);
            }

            return jsonProp;
        }
    }
}
