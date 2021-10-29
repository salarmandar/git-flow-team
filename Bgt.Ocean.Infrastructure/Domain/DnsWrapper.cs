using System.Net;
using static Bgt.Ocean.Infrastructure.Util.EnumGlobalEnvironment;

namespace Bgt.Ocean.Infrastructure.Domain
{
    public interface IDnsWrapper
    {
        string ClientHostName { get; }
    }
    public class DnsWrapper : IDnsWrapper
    {
        public DnsWrapper() { }
        public string ClientHostName
        {
            get
            {
                string result = string.Empty;
                string hostName = EnvironmentSetting.HostName;
                if (!string.IsNullOrEmpty(hostName))
                {
                    result = Dns.GetHostEntry(hostName)?.HostName ?? string.Empty;
                }
                return result;
            }
        }
    }
}
