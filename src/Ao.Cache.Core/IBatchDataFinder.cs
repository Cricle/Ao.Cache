using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IWithDataFinderOptions<TIdentity, TEntity>
    {
        IDataFinderOptions<TIdentity, TEntity> Options { get; set; }
    }
    public interface IBatchDataFinder : IBatchCacheFinder, IBatchPhysicalFinder, IBatchRenewalable
    {
        Task<long> DeleteAsync(IList identity);

        Task<IDictionary<object, bool>> ExistsAsync(IList identity);
    }
    public interface IBatchDataFinder<TIdentity, TEntity> : IWithDataFinderOptions<TIdentity,TEntity>,IBatchCacheFinder<TIdentity, TEntity>, IBatchPhysicalFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>
    {
        Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);

        Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);
    }
}
