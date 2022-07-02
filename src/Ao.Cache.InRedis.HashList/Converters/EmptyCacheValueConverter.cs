using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis.HashList.Converters
{
    internal class EmptyCacheValueConverter: ICacheValueConverter
    {
        public static readonly EmptyCacheValueConverter Instance = new EmptyCacheValueConverter();

        private EmptyCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (RedisValue)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            return value;
        }
    }
}
