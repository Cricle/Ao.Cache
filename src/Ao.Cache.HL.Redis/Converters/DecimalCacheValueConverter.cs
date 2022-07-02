using StackExchange.Redis;


namespace Ao.Cache.HL.Redis.Converters
{
    public class DecimalCacheValueConverter : ICacheValueConverter
    {
        public static readonly DecimalCacheValueConverter Instance = new DecimalCacheValueConverter();

        private DecimalCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double)((decimal)value);
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (decimal)value;
        }
    }
}
