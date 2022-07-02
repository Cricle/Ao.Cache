using StackExchange.Redis;
using Ao.ObjectDesign;
using Ao.Cache.InRedis.HashList;
using Ao.Cache.InRedis.HashList.Converters;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.InRedis.HashList
{
    internal class CacheColumn : ICacheColumn
    {
        public ICacheValueConverter Converter { get; set; }

        public PropertyGetter Getter { get; set; }

        public PropertySetter Setter { get; set; }

        public PropertyInfo Property { get; set; }

        public string Name { get; set; }

        public RedisValue NameRedis { get; set; }

        public string Path{get; set;}

        IReadOnlyList<ICacheColumn> ICacheColumn.Nexts => Nexts;

        public ICacheColumn[] Nexts { get; set; }

        public override string ToString()
        {
            return $"{{Name: {Name}, Path: {Path}}}";
        }
    }
}
