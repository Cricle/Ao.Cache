using MessagePack.Resolvers;
using System;
using MP = MessagePack;

namespace Ao.Cache.Serizlier.MessagePack
{
    public class MessagePackEntityConvertor<TEntity> : MessagePackEntityConvertor, IEntityConvertor<TEntity>
    {
        public static new readonly MessagePackEntityConvertor<TEntity> Default = new MessagePackEntityConvertor<TEntity>();
        public static readonly Type EntityType = typeof(TEntity);

        public byte[] ToBytes(TEntity entry)
        {
            return ToBytes(entry, EntityType);
        }

        public TEntity ToEntry(byte[] bytes)
        {
            return (TEntity)ToEntry(bytes, EntityType);
        }
    }
    public class MessagePackEntityConvertor : IEntityConvertor
    {
        private static readonly MP.MessagePackSerializerOptions defaultOptions = MP.MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance)
            .WithCompression(MP.MessagePackCompression.Lz4Block);

        public static readonly MessagePackEntityConvertor Default = new MessagePackEntityConvertor();

        public MessagePackEntityConvertor()
            : this(defaultOptions)
        {
        }

        public MessagePackEntityConvertor(MP.MessagePackSerializerOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public MP.MessagePackSerializerOptions Options { get; }

        public byte[] ToBytes(object entry, Type type)
        {
            return MP.MessagePackSerializer.Serialize(type, entry, Options);
        }

        public object ToEntry(byte[] bytes, Type type)
        {
            return MP.MessagePackSerializer.Deserialize(type, bytes, Options);
        }
        public object ToEntry(in ReadOnlyMemory<byte> bytes, Type type)
        {
            return MP.MessagePackSerializer.Deserialize(type, bytes, Options);
        }
    }
}
