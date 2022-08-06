using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class EmptyBatchDataFinderFactory<TIdentity, TEntity> : IBatchDataAccesstor<TIdentity, TEntity>
    {
        public Task<IDictionary<TIdentity, TEntity>> FindAsync(IReadOnlyList<TIdentity> identities)
        {
            throw new System.NotImplementedException();
        }
    }
}
