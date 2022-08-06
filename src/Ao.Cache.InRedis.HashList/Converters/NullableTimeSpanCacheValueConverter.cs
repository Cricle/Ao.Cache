using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public class NullableTimeSpanCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableTimeSpanCacheValueConverter Instance = new NullableTimeSpanCacheValueConverter();

        private NullableTimeSpanCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var dt = (TimeSpan?)value;
            if (dt == null)
            {
                return RedisValue.EmptyString;
            }
            return dt.Value.Ticks;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            if (value.TryParse(out long tick))
            {
                return new TimeSpan(tick);
            }
            return CacheValueConverterConst.DoNothing;
        }
    }
}
