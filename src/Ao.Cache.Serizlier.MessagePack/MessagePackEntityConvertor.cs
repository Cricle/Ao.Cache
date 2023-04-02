using MessagePack.Resolvers;
using System;
using System.Text;
using MP = MessagePack;

namespace Ao.Cache.Serizlier.MessagePack
{
    public class MessagePackEntityConvertor : IEntityConvertor
    {
        private static readonly MP.MessagePackSerializerOptions defaultOptions = MP.MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance);

        public static readonly MessagePackEntityConvertor Default = new MessagePackEntityConvertor();

        public MessagePackEntityConvertor()
            : this(defaultOptions,Encoding.UTF8)
        {
        }

        public MessagePackEntityConvertor(MP.MessagePackSerializerOptions options, Encoding encoding)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding { get; }

        public MP.MessagePackSerializerOptions Options { get; }

        public byte[] ToBytes(object entry, Type type)
        {
            return MP.MessagePackSerializer.Serialize(type, entry, Options);
        }
        public object ToEntry(ReadOnlyMemory<byte> bytes, Type type)
        {
            return MP.MessagePackSerializer.Deserialize(type, bytes, Options);
        }
        public object ToEntry(byte[] bytes, Type type)
        {
            return MP.MessagePackSerializer.Deserialize(type, bytes, Options);
        }
        public object ToEntry(in ReadOnlyMemory<byte> bytes, Type type)
        {
            return MP.MessagePackSerializer.Deserialize(type, bytes, Options);
        }

        public string TransferToString(object obj, Type type)
        {
            var bs = ToBytes(obj,type);
            return Encoding.GetString(bs);
        }

        public object TransferFromString(string data, Type type)
        {
            using (var buffer = EncodingHelper.SharedEncoding(data, Encoding))
            {
                return ToEntry(new ReadOnlyMemory<byte>(buffer.Buffers, 0, buffer.Count),type);
            }
        }
    }
}
