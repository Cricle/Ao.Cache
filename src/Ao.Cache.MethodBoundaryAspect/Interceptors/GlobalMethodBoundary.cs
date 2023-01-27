using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public static class GlobalMethodBoundary
    {
        internal static IServiceScopeFactory ServiceScopeFactory;

        public static IServiceScope CreateScope()
        {
            CheckFactory();
            return ServiceScopeFactory.CreateScope();
        }

        public static void CheckFactory()
        {
            if (ServiceScopeFactory == null)
            {
                throw new InvalidOperationException($"Must call IServiceProvider.SetGlobalMethodBoundaryFactory or IServiceScopeFactory.SetGlobalMethodBoundaryFactory first");
            }
        }
    }
}
