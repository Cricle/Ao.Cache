using Ao.Cache.Redis.Finders;
using Ao.Cache.TextJson.Redis;
using Ao.Cache.MessagePack.Redis;
using Ao.Cache.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CacheServiceExtensions
    {
        public static IServiceCollection AddAutoCache(this IServiceCollection services, ServiceLifetime finderLifeTime = ServiceLifetime.Scoped)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(ServiceDescriptor.Describe(typeof(HashCacheFinder<,>), typeof(DefaultHashCacheFinder<,>), finderLifeTime));
            services.Add(ServiceDescriptor.Describe(typeof(ListCacheFinder<,>), typeof(DefaultListCacheFinder<,>), finderLifeTime));
            services.Add(ServiceDescriptor.Describe(typeof(JsonDataFinder<,>), typeof(DefaultRedisJsonDataFinder<,>), finderLifeTime));
            services.Add(ServiceDescriptor.Describe(typeof(MessagePackDataFinder<,>), typeof(DefaultRedisMessagePackDataFinder<,>), finderLifeTime));
            services.Add(ServiceDescriptor.Describe(typeof(InMemoryCacheFinder<,>), typeof(DefaultInMemoryCacheFinder<,>), finderLifeTime));
            return services;
        }
    }
}
