using Ao.Cache;
using Ao.Cache.InRedis;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.IO;

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
            services.AddSingleton<ILockerFactory, RedisLockFactory>();
            services.AddSingleton(typeof(IDataFinderFactory), typeof(RedisDataFinderFactory));
            services.AddScoped(typeof(IDataFinder<,>), typeof(DefaultBitRedisDataFinder<,>));
            services.AddScoped(typeof(IBatchCacheFinder<,>), typeof(DefaultBitRedisBatchFinder<,>));
            return services;
        }
    }
}
