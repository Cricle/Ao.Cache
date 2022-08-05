using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchDataFinder: IBatchCacheFinder, IBatchPhysicalFinder, IBatchRenewalable
    {
        Task<long> DeleteAsync(IList identity);

        Task<IDictionary<object, bool>> ExistsAsync(IList identity);
    }
    public interface IBatchDataFinder<TIdentity, TEntity> : IBatchCacheFinder<TIdentity, TEntity>, IBatchPhysicalFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>
    {        
        Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);

        Task<IDictionary<TIdentity,bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);
    }
    public interface IDataFinderFactory<TIdentity, TEntity>
    {
        IDataFinder<TIdentity, TEntity> Create(IDataAccesstor<TIdentity,TEntity> accesstor);
    }
    public interface IBatchDataFinderFactory<TIdentity, TEntity>
    {
        IBatchDataFinder<TIdentity, TEntity> Create(IBatchDataAccesstor<TIdentity, TEntity> accesstor);
    }
    public static class CreateEmptyFinderExtensions
    {
        public static IDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IDataFinderFactory<TIdentity, TEntity> factory)
        {
            return factory.Create(new EmptyDataFinderFactory<TIdentity, TEntity>());
        }
        public static IBatchDataFinder<TIdentity, TEntity> CreateEmpty<TIdentity, TEntity>(this IBatchDataFinderFactory<TIdentity, TEntity> factory)
        {
            return factory.Create(new EmptyBatchDataFinderFactory<TIdentity, TEntity>());
        }
    }
    public class EmptyDataFinderFactory<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>
    {
        public Task<TEntity> FindAsync(TIdentity identity)
        {
            throw new System.NotImplementedException();
        }
    }
    public class EmptyBatchDataFinderFactory<TIdentity, TEntity> : IBatchDataAccesstor<TIdentity, TEntity>
    {
        public Task<IDictionary<TIdentity, TEntity>> FindAsync(IReadOnlyList<TIdentity> identities)
        {
            throw new System.NotImplementedException();
        }
    }
}
