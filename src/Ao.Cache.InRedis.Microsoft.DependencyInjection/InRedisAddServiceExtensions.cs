using Ao.Cache;
using Ao.Cache.InRedis;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InRedisAddServiceExtensions
    {
        public static IServiceCollection AddDistributedLockFactory(this IServiceCollection services)
        {
            services.AddSingleton<IDistributedLockFactory>(p =>
              new RedLockFactory(new RedLockConfiguration(new ExistingMultiplexersRedLockConnectionProvider
              {
                  Multiplexers = new RedLockMultiplexer[]
                  {
                      new RedLockMultiplexer(p.GetRequiredService<IConnectionMultiplexer>())
                  }
              })));
            return services;
        }
        public static IServiceCollection AddInRedisFinder(this IServiceCollection services)
        {
            services.AddSingleton<ICacheHelperCreator, CacheHelperCreator>();
            services.AddSingleton<IStringMaker, DefaultStringMaker>();
            services.AddSingleton<ILockerFactory, RedisLockFactory>();
            services.AddSingleton<RedisDataFinderFactory>();
            services.AddSingleton<IDataFinderFactory>(x => x.GetRequiredService<RedisDataFinderFactory>());
            services.AddSingleton<IBatchDataFinderFactory>(x => x.GetRequiredService<RedisDataFinderFactory>());
            services.AddSingleton<ISyncDataFinderFactory>(x => x.GetRequiredService<RedisDataFinderFactory>());
            services.AddSingleton<ISyncBatchDataFinderFactory>(x => x.GetRequiredService<RedisDataFinderFactory>());
            services.AddScoped(typeof(IWithDataFinder<,>), typeof(DefaultBitRedisDataFinder<,>));
            services.AddScoped(typeof(IWithBatchDataFinder<,>), typeof(DefaultBitRedisBatchFinder<,>));
            services.AddSingleton(typeof(IDataFinder<,>), typeof(BitRedisDataFinder<,>));
            services.AddSingleton(typeof(IBatchDataFinder<,>), typeof(BitRedisBatchFinder<,>));
            services.AddScoped(typeof(ISyncWithDataFinder<,>), typeof(DefaultSyncBitRedisDataFinder<,>));
            services.AddScoped(typeof(ISyncWithBatchDataFinder<,>), typeof(DefaultSyncBitRedisBatchFinder<,>));
            services.AddSingleton(typeof(ISyncDataFinder<,>), typeof(BitRedisDataFinder<,>));
            services.AddSingleton(typeof(ISyncBatchDataFinder<,>), typeof(BitRedisBatchFinder<,>));
            return services;
        }
    }
}
