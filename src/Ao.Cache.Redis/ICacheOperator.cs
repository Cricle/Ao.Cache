using StackExchange.Redis;

namespace Ao.Cache.Redis
{
    public interface ICacheOperator<TValue>
    {
        void Build();

        void Write(ref object instance, TValue entries);

        TValue As(object value);
    }
}
