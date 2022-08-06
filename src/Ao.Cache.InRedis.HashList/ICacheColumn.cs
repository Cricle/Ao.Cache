using Ao.Cache.InRedis.HashList.Converters;
using Ao.ObjectDesign;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.InRedis.HashList
{
    public interface ICacheColumn
    {
        ICacheValueConverter Converter { get; }

        PropertyGetter Getter { get; }

        PropertySetter Setter { get; }

        PropertyInfo Property { get; }

        string Name { get; }

        RedisValue NameRedis { get; }

        string Path { get; }

        IReadOnlyList<ICacheColumn> Nexts { get; }
    }
}
