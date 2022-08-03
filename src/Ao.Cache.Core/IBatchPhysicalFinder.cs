using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchPhysicalFinder<TIdentity, TEntity>
    {
        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache);
    }
    public interface IBatchPhysicalFinder
    {
        Task<IDictionary> FindInDbAsync(IList identity, bool cache);
    }
}
