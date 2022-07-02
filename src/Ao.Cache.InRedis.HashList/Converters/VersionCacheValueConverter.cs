using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public class VersionCacheValueConverter : ICacheValueConverter
    {
        public static readonly VersionCacheValueConverter Instance = new VersionCacheValueConverter();

        private VersionCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var v = (Version)value;
            if (v == null)
            {
                return RedisValue.EmptyString;
            }
            return v.ToString();
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue || value.IsNullOrEmpty)
            {
                return null;
            }
            return Version.Parse(value);
        }
    }
}
