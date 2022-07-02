using Ao.Cache.InRedis;
using System;
using System.Text.Json;

namespace Ao.Cache.InRedis.TextJson
{
    public class JsonEntityConvertor<TEntity> : JsonEntityConvertor,IEntityConvertor<TEntity>
    {
        public static new readonly JsonEntityConvertor<TEntity> Default = new JsonEntityConvertor<TEntity>();

        public byte[] ToBytes(TEntity entry)
        {
            return ToBytes(entry, typeof(TEntity));
        }

        public TEntity ToEntry(in ReadOnlyMemory<byte> bytes)
        {
            return (TEntity)ToEntry(bytes, typeof(TEntity));
        }
    }
    public class JsonEntityConvertor : IEntityConvertor
    {
        private static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions();

        public static readonly JsonEntityConvertor Default = new JsonEntityConvertor();

        public JsonEntityConvertor()
            :this(defaultOptions)
        {
        }

        public JsonEntityConvertor(JsonSerializerOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public JsonSerializerOptions Options { get; }

        public byte[] ToBytes(object entry, Type type)
        {
            return JsonSerializer.SerializeToUtf8Bytes(entry,type, Options);
        }

        public object ToEntry(in ReadOnlyMemory<byte> bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes.Span, type, Options);
        }
    }

}
