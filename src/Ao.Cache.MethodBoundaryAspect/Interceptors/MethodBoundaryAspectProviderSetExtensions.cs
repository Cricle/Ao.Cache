using Ao.Cache.MethodBoundaryAspect.Interceptors;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MethodBoundaryAspectProviderSetExtensions
    {
        public static IServiceScopeFactory SetGlobalMethodBoundaryFactory(this IServiceScopeFactory factory)
        {
            GlobalMethodBoundary.ServiceScopeFactory = factory;
            return factory;
        }
        public static IServiceProvider SetGlobalMethodBoundaryFactory(this IServiceProvider provider)
        {
            GlobalMethodBoundary.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            return provider;
        }
    }
}
