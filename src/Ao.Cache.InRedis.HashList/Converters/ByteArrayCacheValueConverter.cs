using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public class ByteArrayCacheValueConverter : ICacheValueConverter
    {
        public static readonly ByteArrayCacheValueConverter Instance = new ByteArrayCacheValueConverter();

        private ByteArrayCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (byte[])value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (byte[])value;
        }
    }
}
