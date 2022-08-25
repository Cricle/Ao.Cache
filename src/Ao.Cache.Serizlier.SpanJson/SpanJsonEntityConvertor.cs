using SpanJson;
using System;
using System.Text;

namespace Ao.Cache.Serizlier.SpanJson
{
    public class SpanJsonEntityConvertor : IEntityConvertor
    {
        public static readonly SpanJsonEntityConvertor Default = new SpanJsonEntityConvertor();

        public SpanJsonEntityConvertor()
        {
        }

        public byte[] ToBytes(object entry, Type type)
        {            
            return JsonSerializer.NonGeneric.Utf8.Serialize(entry);
        }

        public object ToEntry(byte[] bytes, Type type)
        {
            return JsonSerializer.NonGeneric.Utf8.Deserialize(bytes, type);
        }
    }
    public class SpanJsonEntityConvertor<TEntity> : IEntityConvertor<TEntity>
    {
        public static readonly SpanJsonEntityConvertor<TEntity> Default = new SpanJsonEntityConvertor<TEntity>();

        public SpanJsonEntityConvertor()
        {
        }

        public byte[] ToBytes(TEntity entry)
        {
            return JsonSerializer.Generic.Utf8.Serialize(entry);
        }
        public TEntity ToEntry(in ReadOnlySpan<byte> bytes)
        {
            return JsonSerializer.Generic.Utf8.Deserialize<TEntity>(bytes);
        }
        public TEntity ToEntry(byte[] bytes)
        {
            return JsonSerializer.Generic.Utf8.Deserialize<TEntity>(bytes);
        }
    }
}
