using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchCacheFinder
    {
        Task<long> SetInCahceAsync(IDictionary pairs);

        Task<IDictionary> FindInCahceAsync(IList identity);
    }
    public interface IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<long> SetInCahceAsync(IDictionary<TIdentity, TEntity> pairs);

        Task<IDictionary<TIdentity, TEntity>> FindInCahceAsync(IReadOnlyList<TIdentity> identity);
    }
}
