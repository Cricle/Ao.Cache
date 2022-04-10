using Ao.Cache.Redis.Converters;
using System;
using System.Reflection;

namespace Ao.Cache.Redis
{
    public interface IConverterProvider
    {
        ICacheValueConverter GetConverter(Type instanceType,PropertyInfo property);
    }
}
