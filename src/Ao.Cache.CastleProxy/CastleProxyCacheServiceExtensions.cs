using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.Proxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CastleProxyCacheServiceExtensions
    {
        public static IServiceCollection WithCastleCacheProxy(this IServiceCollection services)
        {
            services.AddCacheProxy();
            services.TryAddSingleton<CacheInterceptor>();
            services.TryAddSingleton<LockInterceptor>();
            return services;
        }
    }
}
