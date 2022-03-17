using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Converters
{
    public static class KnowsCacheValueConverter
    {
        private static readonly Type StringType = typeof(string);
        private static readonly Type RedisValueType = typeof(RedisValue);
        private static readonly Type CharType = typeof(char);
        private static readonly Type NullableCharType = typeof(char?);
        private static readonly Type ShortType = typeof(short);
        private static readonly Type NullableShortType = typeof(short?);
        private static readonly Type UIntType = typeof(uint);
        private static readonly Type NullableUIntType = typeof(uint?);
        private static readonly Type ULongType = typeof(ulong);
        private static readonly Type NullableULongType = typeof(ulong?);
        private static readonly Type IntType = typeof(int);
        private static readonly Type NullableIntType = typeof(int?);
        private static readonly Type LongType = typeof(long);
        private static readonly Type NullableLongType = typeof(long?);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type NullableDoubleType = typeof(double?);
        private static readonly Type FloatType = typeof(float);
        private static readonly Type NullableFloatType = typeof(float?);
        private static readonly Type DecimalType = typeof(decimal);
        private static readonly Type NullableDecimalType = typeof(decimal?);
        private static readonly Type ByteArrayType = typeof(byte[]);
        private static readonly Type DateTimeType = typeof(DateTime);
        private static readonly Type NullableDateTimeType = typeof(DateTime?);

        public static ICacheValueConverter EndValueConverter { get; set; }

        public static ICacheValueConverter GetConverter(Type type)
        {
            if (type.IsEquivalentTo(StringType))
            {
                return StringCacheValueConverter.Instance;
            }
            if (type.IsEquivalentTo(RedisValueType))
            {
                return EmptyCacheValueConverter.Instance;
            }
            if (type.IsValueType)
            {
                if (type.IsEquivalentTo(CharType))
                {
                    return CharCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableCharType))
                {
                    return NullableCharCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(ShortType))
                {
                    return ShortCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableShortType))
                {
                    return NullableShortCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(UIntType))
                {
                    return UIntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableUIntType))
                {
                    return NullableUIntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(ULongType))
                {
                    return ULongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableULongType))
                {
                    return NullableULongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(IntType))
                {
                    return IntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableIntType))
                {
                    return NullableIntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(LongType))
                {
                    return LongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableLongType))
                {
                    return NullableLongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(DoubleType))
                {
                    return DoubleCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableDoubleType))
                {
                    return NullableDoubleCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(FloatType))
                {
                    return FloatCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableFloatType))
                {
                    return NullableFloatCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(DecimalType))
                {
                    return DecimalCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(NullableDecimalType))
                {
                    return NullableDecimalCacheValueConverter.Instance;
                }                
            }
            if (type.IsEquivalentTo(ByteArrayType))
            {
                return ByteArrayCacheValueConverter.Instance;
            }
            if (type.IsEquivalentTo(DateTimeType))
            {
                return DateTimeCacheValueConverter.Instance;
            }
            if (type.IsEquivalentTo(NullableDateTimeType))
            {
                return NullableDateTimeCacheValueConverter.Instance;
            }
            if (type.IsEnum)
            {
                return EnumCacheValueConverter.Instance;
            }

            return EndValueConverter;
        }
    }
}
