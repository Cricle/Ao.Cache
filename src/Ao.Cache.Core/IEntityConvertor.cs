using System;

namespace Ao.Cache
{
    public interface IEntityConvertor
    {
        byte[] ToBytes(object entry, Type type);

        object ToEntry(ReadOnlyMemory<byte> bytes, Type type);

        object ToEntry(byte[] bytes, Type type);

        string TransferToString(object obj,Type type);

        object TransferFromString(string data,Type type);
    }
}
