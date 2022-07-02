using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IDataAccesstor<TIdentity, TEntry>
    {
        Task<TEntry> FindAsync(TIdentity identity);
    }

}