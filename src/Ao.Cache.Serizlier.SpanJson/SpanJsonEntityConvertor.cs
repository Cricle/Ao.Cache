using SpanJson;
using System;
using System.Text;

namespace Ao.Cache.Serizlier.SpanJson
{
    public class SpanJsonEntityConvertor : IEntityConvertor
    {
        public static readonly SpanJsonEntityConvertor Default = new SpanJsonEntityConvertor(Encoding.UTF8);

        public SpanJsonEntityConvertor(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding { get; }

        public byte[] ToBytes(object entry, Type type)
        {
            return JsonSerializer.NonGeneric.Utf8.Serialize(entry);
        }

        public object ToEntry(byte[] bytes, Type type)
        {
            return JsonSerializer.NonGeneric.Utf8.Deserialize(bytes, type);
        }

        public object ToEntry(ReadOnlyMemory<byte> bytes, Type type)
        {
            return JsonSerializer.NonGeneric.Utf8.Deserialize(bytes.Span, type);
        }

        public object TransferFromString(string data, Type type)
        {
            using (var buffer = EncodingHelper.SharedEncoding(data, Encoding))
            {
                return JsonSerializer.NonGeneric.Utf8.Deserialize(new ReadOnlySpan<byte>(buffer.Buffers, 0, buffer.Count), type);
            }
        }

        public string TransferToString(object obj, Type type)
        {
            var bs = ToBytes(obj,type);
            return Encoding.GetString(bs);
        }
    }
}
