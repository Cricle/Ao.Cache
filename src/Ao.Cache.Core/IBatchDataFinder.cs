using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataFinder<TIdentity, TEntity> : IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache);
        
        Task<IDictionary<TIdentity,bool>> DeleteAsync(IReadOnlyList<TIdentity> identity);

        Task<IDictionary<TIdentity,bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);
    }
}
