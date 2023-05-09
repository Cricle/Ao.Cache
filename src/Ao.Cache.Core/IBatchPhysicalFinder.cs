using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchPhysicalFinder<TIdentity, TEntity>
    {
        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IBatchDataAccesstor<TIdentity,TEntity> batchDataAccesstor,IReadOnlyList<TIdentity> identity, bool cache);
    }
    public interface ISyncBatchPhysicalFinder<TIdentity, TEntity>
    {
        IDictionary<TIdentity, TEntity> FindInDb(ISyncBatchDataAccesstor<TIdentity, TEntity> batchDataAccesstor, IReadOnlyList<TIdentity> identity, bool cache);
    }
}
