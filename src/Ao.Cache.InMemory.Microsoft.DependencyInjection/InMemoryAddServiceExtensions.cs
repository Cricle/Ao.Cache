using Ao.Cache;
using Ao.Cache.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InMemoryAddServiceExtensions
    {
        public static IServiceCollection AddInMemoryFinder(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ILockerFactory, MemoryLockFactory>();
            services.AddSingleton(typeof(IDataFinderFactory), typeof(InMemoryCacheFinderFactory));
            services.AddScoped(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));
            services.AddScoped(typeof(IBatchDataFinder<,>), typeof(DefaultInMemoryBatchCacheFinder<,>));
            return services;
        }
    }
}
