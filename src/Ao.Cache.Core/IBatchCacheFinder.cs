using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<bool> SetInCahceAsync(IEnumerable<KeyValuePair<TIdentity, TEntity>> pairs);

        Task<IDictionary<TIdentity, TEntity>> FindInCahceAsync(IEnumerable<TIdentity> identity);
    }
}
