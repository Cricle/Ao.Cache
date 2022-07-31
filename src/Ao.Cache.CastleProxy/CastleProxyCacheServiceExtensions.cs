using Ao.Cache;
using Ao.Cache.CastleProxy;
using Ao.Cache.CastleProxy.Interceptors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CastleProxyCacheServiceExtensions
    {
        public static IServiceCollection AddCastleCacheProxy(this IServiceCollection services)
        {
            services.AddSingleton<IStringTransfer>(DefaultStringTransfer.Instance);
            services.AddScoped<CacheInterceptor>();
            services.AddScoped<LockInterceptor>();
            services.AddScoped(typeof(IDataAccesstor<,>),typeof(CastleDataAccesstor<,>));
            return services;
        }
    }
}
