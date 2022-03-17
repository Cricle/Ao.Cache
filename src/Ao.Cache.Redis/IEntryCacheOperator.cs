using StackExchange.Redis;

namespace Ao.Cache.Redis
{
    public interface IEntryCacheOperator
    {
        void Build();

        void Write(ref object instance, RedisValue entry);

        RedisValue As(object value);
    }
}
