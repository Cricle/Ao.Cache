using System;

namespace Ao.Cache
{
    public abstract class EntityConvertorBase<TEntity> : IEntityConvertor, IEntityConvertor<TEntity>
    {
        public virtual byte[] ToBytes(TEntity entry)
        {
            return ToBytes(entry, typeof(TEntity));
        }


        public virtual TEntity ToEntry(byte[] bytes)
        {
            return (TEntity)ToEntry(bytes, typeof(TEntity));
        }

        public abstract byte[] ToBytes(object entry, Type type);
        public abstract object ToEntry(byte[] bytes, Type type);
    }
    public interface IEntityConvertor
    {
        byte[] ToBytes(object entry, Type type);

        object ToEntry(byte[] bytes, Type type);
    }
    public interface IEntityConvertor<TEntity>
    {
        byte[] ToBytes(TEntity entry);

        TEntity ToEntry(byte[] bytes);
    }
}
