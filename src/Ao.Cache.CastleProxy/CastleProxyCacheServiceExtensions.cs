﻿using Ao.Cache;
using Ao.Cache.CastleProxy;
using Ao.Cache.CastleProxy.Interceptors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CastleProxyCacheServiceExtensions
    {
        public static IServiceCollection AddCastleCacheProxy(this IServiceCollection services)
        {
            services.AddSingleton<IStringTransfer>(DefaultStringTransfer.Default);
            services.AddSingleton<ICacheNamedHelper, DefaultCacheNamedHelper>();
            services.AddSingleton<CacheInterceptor>();
            services.AddSingleton<LockInterceptor>();
            services.AddSingleton(typeof(AutoCacheService<>));
            return services;
        }
    }
}
