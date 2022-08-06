using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public class DeflateStringCacheValueConverter : ICacheValueConverter
    {
        public static readonly DeflateStringCacheValueConverter Instance = new DeflateStringCacheValueConverter();

        private DeflateStringCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            return CompressionHelper.Deflate(buffer.Buffer, 0, buffer.Length, attr.Level);
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            return attr.Encoding.GetString(CompressionHelper.UnDeflate(value));
        }
    }
}
