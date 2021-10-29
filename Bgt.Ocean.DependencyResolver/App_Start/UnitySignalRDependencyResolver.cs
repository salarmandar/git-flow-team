using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using Unity;

namespace Bgt.Ocean.DependencyResolver.App_Start
{
    public class UnitySignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IUnityContainer _container;

        public UnitySignalRDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            if (_container.IsRegistered(serviceType) || typeof(IHub).IsAssignableFrom(serviceType))
                return _container.Resolve(serviceType);

            return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            if (_container.IsRegistered(serviceType) || typeof(IHub).IsAssignableFrom(serviceType))
                return _container.ResolveAll(serviceType);

            return base.GetServices(serviceType);
        }
    }
}
