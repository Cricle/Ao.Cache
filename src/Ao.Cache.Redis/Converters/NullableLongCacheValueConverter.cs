using StackExchange.Redis;


namespace Ao.Cache.Redis.Converters
{
    public class NullableLongCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableLongCacheValueConverter Instance = new NullableLongCacheValueConverter();

        private NullableLongCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            if (value.TryParse(out long val))
            {
                return val;
            }
            return null;
        }
    }
}
