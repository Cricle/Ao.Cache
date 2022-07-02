using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.HL.Redis.Channel
{
    public interface IRedisSubscriberFactory : IDisposable
    {
        Action<RedisChannel, RedisValue> RegistSubscriber(RedisChannel channel, IRedisSubscriber subscriber);
        Action<RedisChannel, RedisValue> RegistFromServiceSubscriber(RedisChannel channel, Type redisSubscripberType);
        Action<RedisChannel, RedisValue> CreateFromServiceSubscriber(Type redisSubscripberType);
        Action<RedisChannel, RedisValue> CreateSubscriber(IRedisSubscriber subscriber);
    }
}