using StackExchange.Redis;


namespace Ao.Cache.HL.Redis.Converters
{
    public class IntCacheValueConverter : ICacheValueConverter
    {
        public static readonly IntCacheValueConverter Instance = new IntCacheValueConverter();

        private IntCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (value .TryParse(out int val))
            {
                return val;
            }
            return default(int);
        }
    }
}
