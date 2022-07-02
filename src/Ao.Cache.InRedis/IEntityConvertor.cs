using System;

namespace Ao.Cache.InRedis
{
    public interface IEntityConvertor
    {
        byte[] ToBytes(object entry, Type type);

        object ToEntry(in ReadOnlyMemory<byte> bytes, Type type);
    }
    public interface IEntityConvertor<TEntity>
    {
        byte[] ToBytes(TEntity entry);

        TEntity ToEntry(in ReadOnlyMemory<byte> bytes);
    }
}
