using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchCacheFinder<TIdentity, TEntry>
    {
        Task<bool> SetInCahceAsync(IEnumerable<KeyValuePair<TIdentity, TEntry>> pairs);

        Task<IDictionary<TIdentity, TEntry>> FindInCahceAsync(IEnumerable<TIdentity> identity);
    }
}
