using MessagePack.Resolvers;
using System;
using MP = MessagePack;

namespace Ao.Cache.MessagePack.Redis
{
    public abstract class MessagePackDataFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        private static readonly MP.MessagePackSerializerOptions defaultOptions = MP.MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance)
            .WithCompression(MP.MessagePackCompression.Lz4Block);
        
        protected byte[] ToBytes(TEntry entry)
        {
            var options=GetOptions();
            return MP.MessagePackSerializer.Serialize(entry,options);
        }
        protected TEntry ToEntry(in ReadOnlyMemory<byte> bytes)
        {
            var options = GetOptions();
            return MP.MessagePackSerializer.Deserialize<TEntry>(bytes, options);
        }
        protected virtual MP.MessagePackSerializerOptions GetOptions()
        {
            return defaultOptions;
        }
    }
}
