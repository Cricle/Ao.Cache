
using StackExchange.Redis;

namespace Ao.Cache.Redis.Converters
{
    public class ULongCacheValueConverter : ICacheValueConverter
    {
        public static readonly ULongCacheValueConverter Instance = new ULongCacheValueConverter();

        private ULongCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (ulong)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (ulong)value;
        }
    }
}
