using StackExchange.Redis;


namespace Ao.Cache.Redis.Converters
{
    public class NullableFloatCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableFloatCacheValueConverter Instance = new NullableFloatCacheValueConverter();

        private NullableFloatCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (float?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (float?)value;
        }
    }
}
