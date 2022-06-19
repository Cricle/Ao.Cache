using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataAccesstorBase<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>
    {
        public static readonly TimeSpan DefaultCacheTime = TimeSpan.FromSeconds(3.0);

        public static readonly string EntryFriendlyName = TypeNameHelper.GetFriendlyFullName(typeof(TEntity));

        public bool CanRenewal(TIdentity identity)
        {
            return true;
        }

        public abstract Task<TEntity> FindAsync(TIdentity identity);

        public virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DefaultCacheTime;
        }

        public virtual string GetHead()
        {
            return null;
        }

        public virtual string GetPart(TIdentity identity)
        {
           return identity?.ToString();
        }
    }
}
