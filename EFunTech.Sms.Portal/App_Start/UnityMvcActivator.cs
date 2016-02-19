using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using System.Web.Http;
using EFunTech.Sms.Portal.Controllers.Common;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Practices.ServiceLocation;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(EFunTech.Sms.Portal.App_Start.UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(EFunTech.Sms.Portal.App_Start.UnityWebActivator), "Shutdown")]

namespace EFunTech.Sms.Portal.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            // http://stackoverflow.com/questions/16763994/web-api-is-not-call-my-dependency-resolver

            // set resolver for mvc (System.Web.Mvc)
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            // set resolver for Web API (System.Web.Http)
            // http://www.asp.net/web-api/overview/advanced/dependency-injection
            GlobalConfiguration.Configuration.DependencyResolver = new WebApiDependencyResolver(container);

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            // Uncomment if you want to use PerRequestLifetimeManager
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }

    public class WebApiDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        protected IUnityContainer container;

        public WebApiDependencyResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new WebApiDependencyResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}