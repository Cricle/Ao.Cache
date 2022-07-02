using StackExchange.Redis;


namespace Ao.Cache.InRedis.HashList.Converters
{
    public class DoubleCacheValueConverter : ICacheValueConverter
    {
        public static readonly DoubleCacheValueConverter Instance = new DoubleCacheValueConverter();

        private DoubleCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (double)value;
        }
    }
}
