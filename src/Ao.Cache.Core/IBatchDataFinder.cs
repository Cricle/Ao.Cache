using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataFinder: IBatchCacheFinder, IBatchPhysicalFinder
    {
        Task<long> DeleteAsync(IList identity);

        Task<IDictionary<object, bool>> ExistsAsync(IList identity);
    }
    public interface IBatchDataFinder<TIdentity, TEntity> : IBatchCacheFinder<TIdentity, TEntity>, IBatchPhysicalFinder<TIdentity, TEntity>
    {        
        Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);

        Task<IDictionary<TIdentity,bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);
    }
}
