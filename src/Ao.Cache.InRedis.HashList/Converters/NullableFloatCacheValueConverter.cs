using StackExchange.Redis;
using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public class StructCacheValueConverter : ICacheValueConverter
    {
        public static readonly StructCacheValueConverter Instance = new StructCacheValueConverter();

        private StructCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            var propType = column.Property.PropertyType;
            if (!propType.IsValueType)
            {
                throw new InvalidOperationException($"Can't convert {propType} to struct");
            }
            var buffer = GetBytes(value, propType);
            return buffer;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            var buffer = (byte[])value;
            return FromBytes(buffer, column.Property.PropertyType);
        }

        private static ReadOnlyMemory<byte> GetBytes(object val, Type type)
        {
            var size = Marshal.SizeOf(type);
            var buffer = ArrayPool<byte>.Shared.Rent(size);
            try
            {
                var ptr = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(val, ptr, false);
                    Marshal.Copy(ptr, buffer, 0, size);
                    return new ReadOnlyMemory<byte>(buffer, 0, size);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

        }
        private static object FromBytes(byte[] arr, Type type)
        {
            var size = Marshal.SizeOf(type);
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(arr, 0, ptr, size);

                return Marshal.PtrToStructure(ptr, type);

            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
    public class NullableFloatCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableFloatCacheValueConverter Instance = new NullableFloatCacheValueConverter();

        private NullableFloatCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (float?)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (float?)value;
        }
    }
}
