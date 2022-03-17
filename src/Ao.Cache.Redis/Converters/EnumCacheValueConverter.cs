using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ao.Cache.Redis.Converters
{
    public class EnumCacheValueConverter : ICacheValueConverter
    {
        public static readonly EnumCacheValueConverter Instance = new EnumCacheValueConverter();

        private EnumCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return value.ToString();
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            var val = value.ToString();
            var helper = EnumHelper.GetEnumHelper(column.Property.PropertyType);
            if (helper.TryConvert(val,out var enumVal))
            {
                return enumVal;
            }
            return CacheValueConverterConst.DoNothing;
        }
        class EnumHelper
        {
            private static readonly Dictionary<Type,EnumHelper> enumHelpers = new Dictionary<Type,EnumHelper>();

            public static EnumHelper GetEnumHelper(Type type)
            {
                if (!type.IsEnum)
                {
                    throw new ArgumentException($"Type {type} is not enum");
                }
                if (!enumHelpers.TryGetValue(type,out var helper))
                {
                    helper = new EnumHelper(type);
                    enumHelpers[type]=helper;
                }
                return helper;
            }

            private EnumHelper(Type target)
            {
                map = Enum.GetNames(target).ToDictionary(x => x,
                    x => Enum.Parse(target, x), StringComparer.OrdinalIgnoreCase);
                FirstValue = map.Values.FirstOrDefault();
                if (FirstValue==null)
                {
                    FirstValue = 0;
                }
            }

            private readonly Dictionary<string, object> map;

            public object FirstValue { get; }

            public bool TryConvert(string name,out object value)
            {
                return map.TryGetValue(name, out value);
            }
        }
    }
}
