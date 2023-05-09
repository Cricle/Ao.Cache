using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataAccesstor<TIdentity, TEntry>
    {
        Task<IDictionary<TIdentity, TEntry>> FindAsync(IReadOnlyList<TIdentity> identities);
    }
    public interface ISyncBatchDataAccesstor<TIdentity, TEntry>
    {
        IDictionary<TIdentity, TEntry> Find(IReadOnlyList<TIdentity> identities);
    }
}