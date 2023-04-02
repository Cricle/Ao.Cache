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
            return Encoding.GetBytes(ToString(entry, type));
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
            var bytes = Encoding.GetBytes(str);
            return JsonSerializer.Deserialize(bytes, type, Options);
        }

        public string TransferToString(object obj, Type type)
        {
            return ToString(obj, type);
        }

        public object TransferFromString(string data, Type type)
        {
            return ToEntry(data, type);
        }

        public object ToEntry(ReadOnlyMemory<byte> bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes.Span, type, Options);
        }
    }
}
