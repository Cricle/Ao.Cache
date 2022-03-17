using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataFinder<TIdentity, TEntry>: IBatchCacheFinder<TIdentity,TEntry>
    {
        Task<IDictionary<TIdentity, TEntry>> FindInDbAsync(IEnumerable<TIdentity> identity, bool cache);
    }
}
