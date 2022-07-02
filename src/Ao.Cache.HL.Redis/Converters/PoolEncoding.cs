using System;
using System.Buffers;
using System.Text;

namespace Ao.Cache.HL.Redis.Converters
{
    public static class PoolEncoding
    {
        public static PoolBuffer GetBytes(string s)
        {
            return GetBytes(s, Encoding.UTF8, ArrayPool<byte>.Shared);
        }
        public static PoolBuffer GetBytes(string s, Encoding encoding)
        {
            return GetBytes(s, encoding, ArrayPool<byte>.Shared);
        }
        public static PoolBuffer GetBytes(string s,Encoding encoding,ArrayPool<byte> pool)
        {
            if (s==null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (encoding is null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (pool is null)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            var byteCount = encoding.GetByteCount(s);
            var bytes = pool.Rent(byteCount);
            int bytesReceived = encoding.GetBytes(s, 0, s.Length, bytes, 0);
            return new PoolBuffer(pool,bytes, bytesReceived);
        }
    }
}
