using StackExchange.Redis;


namespace Ao.Cache.InRedis.HashList.Converters
{
    public class NullableCharCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableCharCacheValueConverter Instance = new NullableCharCacheValueConverter();

        private NullableCharCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int?)(char?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (char?)(int?)value;
        }
    }
}
