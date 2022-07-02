using StackExchange.Redis;


namespace Ao.Cache.HL.Redis.Converters
{
    public class NullableBoolCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableBoolCacheValueConverter Instance = new NullableBoolCacheValueConverter();

        private NullableBoolCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (bool?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (bool?)value;
        }
    }
}
