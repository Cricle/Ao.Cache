using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public class GlobalMethodBoundary
    {
        public static IServiceScopeFactory ServiceScopeFactory;

        public static IServiceScope CreateScope()
        {
            CheckFactory();
            return ServiceScopeFactory.CreateScope();
        }

        public static void CheckFactory()
        {
            if (ServiceScopeFactory == null)
            {
                throw new InvalidOperationException($"GlobalMethodBoundary.ServiceScopeFactory is null");
            }
        }
    }
}
