using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList
{
    public interface ICacheOperator<TValue>
    {
        void Build();

        void Write(ref object instance, TValue entries);

        TValue As(object value);
    }
}
