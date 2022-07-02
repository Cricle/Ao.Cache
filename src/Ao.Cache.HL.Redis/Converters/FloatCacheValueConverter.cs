using StackExchange.Redis;


namespace Ao.Cache.HL.Redis.Converters
{
    public class FloatCacheValueConverter : ICacheValueConverter
    {
        public static readonly FloatCacheValueConverter Instance = new FloatCacheValueConverter();

        private FloatCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (float)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (float)value;
        }
    }
}
