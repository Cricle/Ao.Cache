using Ao.Cache;
using Ao.Cache.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InMemoryAddServiceExtensions
    {
        public static IServiceCollection AddInMemoryFinder(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheHelperCreator, CacheHelperCreator>();
            services.AddSingleton<IStringMaker, DefaultStringMaker>();
            services.AddSingleton<ILockerFactory, MemoryLockFactory>();
            services.AddSingleton<InMemoryCacheFinderFactory>();
            services.AddSingleton<IDataFinderFactory>(x => x.GetRequiredService<InMemoryCacheFinderFactory>());
            services.AddSingleton<IBatchDataFinderFactory>(x => x.GetRequiredService<InMemoryCacheFinderFactory>());
            services.AddSingleton<ISyncDataFinderFactory>(x => x.GetRequiredService<InMemoryCacheFinderFactory>());
            services.AddSingleton<ISyncBatchDataFinderFactory>(x => x.GetRequiredService<InMemoryCacheFinderFactory>());
            services.AddScoped(typeof(IDataFinder<,>), typeof(InMemoryCacheFinder<,>));
            services.AddScoped(typeof(IBatchDataFinder<,>), typeof(InMemoryBatchCacheFinder<,>));
            services.AddScoped(typeof(IWithDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));
            services.AddScoped(typeof(IWithDataFinder<,>), typeof(DefaultInMemoryBatchCacheFinder<,>));
            services.AddScoped(typeof(ISyncDataFinder<,>), typeof(InMemoryCacheFinder<,>));
            services.AddScoped(typeof(ISyncBatchDataFinder<,>), typeof(InMemoryBatchCacheFinder<,>));
            services.AddScoped(typeof(ISyncWithDataFinder<,>), typeof(DefaultSyncInMemoryCacheFinder<,>));
            services.AddScoped(typeof(ISyncWithDataFinder<,>), typeof(DefaultSyncInMemoryBatchCacheFinder<,>));
            return services;
        }
    }
}
