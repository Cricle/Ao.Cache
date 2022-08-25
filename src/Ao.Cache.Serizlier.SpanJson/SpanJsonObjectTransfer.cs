using System;
using System.Text;

namespace Ao.Cache.Serizlier.SpanJson
{
    public class SpanJsonObjectTransfer : IObjectTransfer
    {
        public static readonly SpanJsonObjectTransfer Default = new SpanJsonObjectTransfer(Encoding.UTF8);

        public SpanJsonObjectTransfer(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding { get; }

        public byte[] Transfer<T>(T obj)
        {
            return SpanJsonEntityConvertor<T>.Default.ToBytes(obj);
        }

        public T Transfer<T>(byte[] data)
        {
            return SpanJsonEntityConvertor<T>.Default.ToEntry(data);
        }

        public T TransferByString<T>(string data)
        {
            using (var buffer = EncodingHelper.SharedEncoding(data, Encoding))
            {
                return SpanJsonEntityConvertor<T>.Default.ToEntry(new ReadOnlySpan<byte>(buffer.Buffers, 0, buffer.Count));
            }
        }

        public string TransferToString<T>(T obj)
        {
            var bs= SpanJsonEntityConvertor<T>.Default.ToBytes(obj);
            return Encoding.GetString(bs);
        }
    }
}
