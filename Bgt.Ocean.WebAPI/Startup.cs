using Bgt.Ocean.DependencyResolver.App_Start;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(Bgt.Ocean.WebAPI.Startup))]
namespace Bgt.Ocean.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            string conn = ConfigurationManager.ConnectionStrings["SignalRConnection"].ConnectionString;

            var resolver = new UnitySignalRDependencyResolver(UnityConfig.Container);
            resolver.UseSqlServer(conn);

            UnityConfig.RegisterSignalRResolver(resolver);

            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(new HubConfiguration
            {
                Resolver = resolver
            });
        }
    }
}