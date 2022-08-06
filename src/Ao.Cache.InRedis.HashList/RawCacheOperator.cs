using Ao.Cache.InRedis.HashList.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Ao.Cache.InRedis.HashList
{
    public class RawCacheOperator : EntryCacheOperator
    {
        private static readonly Dictionary<Type, RawCacheOperator> defaultRedisOpCache = new Dictionary<Type, RawCacheOperator>();

        public static RawCacheOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new RawCacheOperator(type);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        private ICacheValueConverter converter;

        public RawCacheOperator(Type target) : base(target)
        {
        }

        public override void Build()
        {
            base.Build();
            converter = KnowsCacheValueConverter.GetConverter(Target);
        }

        protected override void WriteCore(ref object instance, in RedisValue entry)
        {
            instance = converter.ConvertBack(entry, null);
        }
        protected override RedisValue AsCore(object value)
        {
            return converter.Convert(null, value, null);
        }
    }
}
