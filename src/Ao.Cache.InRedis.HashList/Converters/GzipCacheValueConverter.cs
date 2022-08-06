using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public class GzipCacheValueConverter : ICacheValueConverter
    {
        public static readonly GzipCacheValueConverter Instance = new GzipCacheValueConverter();


        private GzipCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var buffer = (byte[])value;
            return CompressionHelper.Gzip(buffer, attr.Level);
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            var buffer = (byte[])value;
            return CompressionHelper.UnGzip(buffer);
        }
    }
}
