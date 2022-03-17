using StackExchange.Redis;


namespace Ao.Cache.Redis.Converters
{
    public class LongCacheValueConverter : ICacheValueConverter
    {
        public static readonly LongCacheValueConverter Instance = new LongCacheValueConverter();

        private LongCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (value.TryParse(out long val))
            {
                return val;
            }
            return default(long);
        }
    }
}
