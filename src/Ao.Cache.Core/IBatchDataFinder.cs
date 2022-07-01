using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataFinder<TIdentity, TEntity> : IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IEnumerable<TIdentity> identity, bool cache);
        
        Task<IDictionary<TIdentity,bool>> DeleteAsync(IEnumerable<TIdentity> identity);

        Task<IDictionary<TIdentity,bool>> ExistsAsync(IEnumerable<TIdentity> identity);
    }
}
