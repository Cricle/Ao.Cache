using System;
using System.Buffers;

namespace Ao.Cache.InRedis.HashList.Converters
{
    public readonly struct PoolBuffer : IDisposable
    {
        public readonly ArrayPool<byte> Pool;

        public readonly byte[] Buffer;

        public readonly int Length;

        internal PoolBuffer(ArrayPool<byte> pool, byte[] buffer, int length)
        {
            Pool = pool;
            Buffer = buffer;
            Length = length;
        }

        public void Dispose()
        {
            Pool.Return(Buffer);
        }
    }
}
