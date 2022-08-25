using System;
using System.Text;

namespace Ao.Cache.Serizlier.MessagePack
{
    public class MessagePackObjectTransfer : IObjectTransfer
    {
        public static readonly MessagePackObjectTransfer Default = new MessagePackObjectTransfer(Encoding.UTF8);

        public MessagePackObjectTransfer(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding { get; }

        public byte[] Transfer<T>(T obj)
        {
            return MessagePackEntityConvertor<T>.Default.ToBytes(obj);
        }

        public T Transfer<T>(byte[] data)
        {
            return MessagePackEntityConvertor<T>.Default.ToEntry(data);
        }

        public T TransferByString<T>(string data)
        {
            using (var buffer = EncodingHelper.SharedEncoding(data, Encoding))
            {
                return (T)MessagePackEntityConvertor<T>.Default
                    .ToEntry(new ReadOnlyMemory<byte>(buffer.Buffers, 0, buffer.Count), typeof(T));
            }
        }

        public string TransferToString<T>(T obj)
        {
            var bs = MessagePackEntityConvertor<T>.Default.ToBytes(obj);
            return Encoding.GetString(bs);
        }
    }
}
