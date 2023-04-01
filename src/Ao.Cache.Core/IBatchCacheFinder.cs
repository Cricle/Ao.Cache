using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs);

        Task<IDictionary<TIdentity, TEntity>> FindInCacheAsync(IReadOnlyList<TIdentity> identity);
    }
}
