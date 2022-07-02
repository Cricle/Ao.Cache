using StackExchange.Redis;


namespace Ao.Cache.InRedis.HashList.Converters
{
    public class NullableUIntCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableUIntCacheValueConverter Instance = new NullableUIntCacheValueConverter();

        private NullableUIntCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (uint?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (uint?)value;
        }
    }
}
