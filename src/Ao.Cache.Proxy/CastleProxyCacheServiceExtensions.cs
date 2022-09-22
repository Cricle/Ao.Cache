using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ao.Cache.Proxy
{
    public static class ProxyCacheServiceExtensions
    {
        public static IServiceCollection AddCacheProxy(this IServiceCollection services)
        {
            services.TryAddSingleton<IStringTransfer>(DefaultStringTransfer.Default);
            services.TryAddSingleton<ICacheNamedHelper>(DefaultCacheNamedHelper.Default);
            services.TryAddSingleton<AutoCacheService>();
            return services;
        }
    }
}
