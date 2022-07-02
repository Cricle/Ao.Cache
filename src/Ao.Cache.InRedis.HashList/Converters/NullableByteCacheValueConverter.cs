using StackExchange.Redis;


namespace Ao.Cache.InRedis.HashList.Converters
{
    public class NullableByteCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableByteCacheValueConverter Instance = new NullableByteCacheValueConverter();

        private NullableByteCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long?)(byte?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (byte?)(long?)value;
        }
    }
}
