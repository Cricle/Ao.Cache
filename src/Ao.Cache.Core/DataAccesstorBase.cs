using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataAccesstorBase<TIdentity, TEntry> : IDataAccesstor<TIdentity, TEntry>
    {
        public static readonly TimeSpan DefaultCacheTime = TimeSpan.FromSeconds(3.0);

        public abstract Task<TEntry> FindAsync(TIdentity identity);

        public virtual TimeSpan? GetCacheTime(TIdentity identity, TEntry entity)
        {
            return DefaultCacheTime;
        }
    }
}
