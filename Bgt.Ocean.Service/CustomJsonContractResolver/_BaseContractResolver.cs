using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.CustomJsonContractResolver
{
    public abstract class ContractResolver : DefaultContractResolver
    {
        protected readonly IEnumerable<IContractResolver> _resolver;

        protected ContractResolver()
        {
            _resolver = new List<IContractResolver>()
            {
                new IgnoreResolver(),
                new NameResolver()
            };
        }
    }
}
