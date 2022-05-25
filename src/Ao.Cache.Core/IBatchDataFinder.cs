using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataFinder<TIdentity, TEntity>: IBatchCacheFinder<TIdentity,TEntity>
    {
        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IEnumerable<TIdentity> identity, bool cache);
    }
}
