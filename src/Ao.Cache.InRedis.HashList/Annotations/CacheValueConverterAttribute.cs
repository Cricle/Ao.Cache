using Ao.Cache.InRedis.HashList.Converters;
using System;

namespace Ao.Cache.InRedis.HashList.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CacheValueConverterAttribute : Attribute
    {
        private static readonly string CacheValueConverterName = typeof(ICacheValueConverter).FullName;

        public CacheValueConverterAttribute(Type convertType)
        {
            ConvertType = convertType ?? throw new ArgumentNullException(nameof(convertType));
            if (convertType.GetInterface(CacheValueConverterName) == null)
            {
                throw new ArgumentException($"Type {convertType} is not implement {CacheValueConverterName}");
            }
        }

        public Type ConvertType { get; }
    }
}
