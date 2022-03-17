
using StackExchange.Redis;

namespace Ao.Cache.Redis.Converters
{
    public class UIntCacheValueConverter : ICacheValueConverter
    {
        public static readonly UIntCacheValueConverter Instance = new UIntCacheValueConverter();

        private UIntCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (uint)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (uint)value;
        }
    }
}
