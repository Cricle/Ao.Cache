using StackExchange.Redis;


namespace Ao.Cache.Redis.Converters
{

    public class CharCacheValueConverter : ICacheValueConverter
    {
        public static readonly CharCacheValueConverter Instance = new CharCacheValueConverter();

        private CharCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(char)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (char)(long)value;
        }
    }
}
