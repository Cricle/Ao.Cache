using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Channel
{
    public class RedisSubscriberFactory : IRedisSubscriberFactory, IDisposable
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IConnectionMultiplexer connection;
        private readonly List<(RedisChannel, Action<RedisChannel, RedisValue>)> subscribers =
            new List<(RedisChannel, Action<RedisChannel, RedisValue>)>();

        public RedisSubscriberFactory(IServiceScopeFactory serviceScopeFactory,
            IConnectionMultiplexer connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }
        public Action<RedisChannel, RedisValue> RegistSubscriber(RedisChannel channel, IRedisSubscriber subscriber)
        {
            var val = CreateSubscriber(subscriber);
            subscribers.Add((channel, val));
            connection.GetSubscriber().Subscribe(channel, val);
            return val;
        }
        public Action<RedisChannel, RedisValue> RegistFromServiceSubscriber(RedisChannel channel, Type redisSubscripberType)
        {
            var val = CreateFromServiceSubscriber(redisSubscripberType);
            subscribers.Add((channel, val));
            connection.GetSubscriber().Subscribe(channel, val);
            return val;
        }
        
        private Action<RedisChannel, RedisValue> CheckSubscribeType(Type redisSubscriberType)
        {
            if (redisSubscriberType is null)
            {
                throw new ArgumentNullException(nameof(redisSubscriberType));
            }
            if (redisSubscriberType.GetInterface(typeof(IRedisSubscriber).FullName) == null)
            {
                throw new ArgumentException($"Type {redisSubscriberType.FullName} is not implement {typeof(IRedisSubscriber).FullName}!");
            }
            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var sub = scope.ServiceProvider.GetRequiredService(redisSubscriberType) as IRedisSubscriber;
                await sub.DoAsync(c, v, scope.ServiceProvider);
            });
            return val;
        }
        public Action<RedisChannel, RedisValue> CreateFromServiceSubscriber(Type redisSubscriberType)
        {
            CheckSubscribeType(redisSubscriberType);
            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var sub = (IRedisSubscriber)scope.ServiceProvider.GetRequiredService(redisSubscriberType);
                await sub.DoAsync(c, v, scope.ServiceProvider);
            });
            return val;
        }
        public Action<RedisChannel, RedisValue> CreateSubscriber(IRedisSubscriber subscriber)
        {
            if (subscriber is null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                await subscriber.DoAsync(c, v, scope.ServiceProvider);
            });
            return val;
        }

        public void Dispose()
        {
            var sub = connection.GetSubscriber();
            foreach (var item in subscribers)
            {
                sub.Unsubscribe(item.Item1, item.Item2);
            }
            subscribers.Clear();
        }
    }
}
