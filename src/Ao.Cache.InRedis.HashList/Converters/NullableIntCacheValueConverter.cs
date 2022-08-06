using StackExchange.Redis;


namespace Ao.Cache.InRedis.HashList.Converters
{
    public class NullableIntCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableIntCacheValueConverter Instance = new NullableIntCacheValueConverter();

        private NullableIntCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            if (value.TryParse(out int val))
            {
                return val;
            }
            return null;
        }
    }
}
