using Ao.Cache.HL.Redis.Converters;
using System;
using System.Reflection;

namespace Ao.Cache.HL.Redis
{
    public interface IConverterProvider
    {
        ICacheValueConverter GetConverter(Type instanceType,PropertyInfo property);
    }
}
