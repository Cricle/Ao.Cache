using StackExchange.Redis;


namespace Ao.Cache.Redis.Converters
{
    public class NullableDecimalCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableDecimalCacheValueConverter Instance = new NullableDecimalCacheValueConverter();

        private NullableDecimalCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double?)((decimal?)value);
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (decimal?)value;
        }
    }
}
