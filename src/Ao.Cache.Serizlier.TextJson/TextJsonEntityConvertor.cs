using System;
using System.Text;
using System.Text.Json;

namespace Ao.Cache.Serizlier.TextJson
{
    public class TextJsonEntityConvertor : IEntityConvertor
    {
        internal static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions();

        public static readonly TextJsonEntityConvertor Default = new TextJsonEntityConvertor();

        public TextJsonEntityConvertor()
            : this(defaultOptions, Encoding.UTF8)
        {
        }

        public TextJsonEntityConvertor(JsonSerializerOptions options, Encoding encoding)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public JsonSerializerOptions Options { get; }

        public Encoding Encoding { get; }

        public byte[] ToBytes(object entry, Type type)
        {
            return Encoding.GetBytes(ToString(entry,type));
        }
        public string ToString(object entry, Type type)
        {
            return JsonSerializer.Serialize(entry, type, Options);
        }
        public object ToEntry(byte[] bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes, type, Options);
        }
        public object ToEntry(string str, Type type)
        {
            var bytes=Encoding.GetBytes(str);
            return JsonSerializer.Deserialize(bytes, type, Options);
        }
    }

    public class TextJsonEntityConvertor<TEntity> : TextJsonEntityConvertor, IEntityConvertor<TEntity>
    {
        public static new readonly TextJsonEntityConvertor<TEntity> Default = new TextJsonEntityConvertor<TEntity>();

        public byte[] ToBytes(TEntity entry)
        {
            return ToBytes(entry, typeof(TEntity));
        }
        public string ToString(TEntity entry)
        {
            return ToString(entry, typeof(TEntity));
        }
        public TEntity ToEntry(byte[] bytes)
        {
            return (TEntity)ToEntry(bytes, typeof(TEntity));
        }
        public TEntity ToEntry(string str)
        {
            return (TEntity)ToEntry(str, typeof(TEntity));
        }
    }

}
