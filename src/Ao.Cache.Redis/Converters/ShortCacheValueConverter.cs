using StackExchange.Redis;


namespace Ao.Cache.Redis.Converters
{
    public class ShortCacheValueConverter : ICacheValueConverter
    {
        public static readonly ShortCacheValueConverter Instance = new ShortCacheValueConverter();

        private ShortCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(short)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (short)(long)value;
        }
    }
}
