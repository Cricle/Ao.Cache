using Ao.Cache.InRedis.HashList.Converters;
using System;
using System.Reflection;

namespace Ao.Cache.InRedis.HashList
{
    public interface IConverterProvider
    {
        ICacheValueConverter GetConverter(Type instanceType, PropertyInfo property);
    }
}
