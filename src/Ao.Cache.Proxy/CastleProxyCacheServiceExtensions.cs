using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Proxy
{
    public static class CastleProxyCacheServiceExtensions
    {
        public static IServiceCollection AddCastleCacheProxy(this IServiceCollection services)
        {
            services.AddSingleton<IStringTransfer>(DefaultStringTransfer.Default);
            services.AddSingleton<ICacheNamedHelper>(DefaultCacheNamedHelper.Default);
            //services.AddSingleton<CacheInterceptor>();
            //services.AddSingleton<LockInterceptor>();
            services.AddSingleton<AutoCacheService>();
            return services;
        }
    }
}
