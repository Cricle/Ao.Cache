using StackExchange.Redis;


namespace Ao.Cache.HL.Redis.Converters
{
    public class NullableShortCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableShortCacheValueConverter Instance = new NullableShortCacheValueConverter();

        private NullableShortCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (short?)(char?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (short?)(int?)value;
        }
    }
}
