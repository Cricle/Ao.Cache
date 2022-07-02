using StackExchange.Redis;
using System;

namespace Ao.Cache.HL.Redis.Converters
{
    public class TimeSpanCacheValueConverter : ICacheValueConverter
    {
        public static readonly TimeSpanCacheValueConverter Instance = new TimeSpanCacheValueConverter();

        private TimeSpanCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var dt = (TimeSpan)value;
            return dt.Ticks;
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
