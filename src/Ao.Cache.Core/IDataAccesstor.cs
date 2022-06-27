using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IDataAccesstor<TIdentity, TEntry>
    {
        Task<TEntry> FindAsync(TIdentity identity);

        TimeSpan? GetCacheTime(TIdentity identity, TEntry entity);

        string GetHead();

        string GetPart(TIdentity identity);

        bool CanRenewal(TIdentity identity);
    }

}