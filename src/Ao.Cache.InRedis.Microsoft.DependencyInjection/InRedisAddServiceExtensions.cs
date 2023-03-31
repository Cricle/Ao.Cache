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
            services.AddScoped(typeof(IDataFinder<,>), typeof(DefaultBitRedisDataFinder<,>));
            services.AddScoped(typeof(IBatchDataFinder<,>), typeof(DefaultBitRedisBatchFinder<,>));
            return services;
        }
    }
}
