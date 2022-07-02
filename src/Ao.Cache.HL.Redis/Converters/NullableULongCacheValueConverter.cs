using StackExchange.Redis;


namespace Ao.Cache.HL.Redis.Converters
{
    public class NullableULongCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableULongCacheValueConverter Instance = new NullableULongCacheValueConverter();

        private NullableULongCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (ulong?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (ulong?)value;
        }
    }
}
