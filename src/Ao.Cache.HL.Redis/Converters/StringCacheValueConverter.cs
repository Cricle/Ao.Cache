using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.HL.Redis.Converters
{
    public class StringCacheValueConverter : ICacheValueConverter
    {
        public static readonly StringCacheValueConverter Instance = new StringCacheValueConverter();
        
        private StringCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (string)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            return value.ToString();
        }
    }
}
