using StackExchange.Redis;

using System;

namespace Ao.Cache.HL.Redis.Converters
{
    public class NullableDateTimeCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableDateTimeCacheValueConverter Instance = new NullableDateTimeCacheValueConverter();

        private NullableDateTimeCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var dt = (DateTime?)value;
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
                return new DateTime(tick);
            }
            return null;
        }
    }
}
