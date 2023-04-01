using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IWithBatchDataAccesstorFinder<TIdentity, TEntity>
    {
        IBatchDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache);
    }
}
