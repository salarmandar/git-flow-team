using Bgt.Ocean.Service.ModelViews.Systems;
using Newtonsoft.Json;
using System.Net.Http;

namespace Bgt.Ocean.WebAPI.Extends
{
    public class HttpSystemMessage : HttpResponseMessage
    {

        public HttpSystemMessage(SystemMessageView message) : base()
        {
            Headers.Add(nameof(SystemMessageView), JsonConvert.SerializeObject(message));
        }
    }
}
