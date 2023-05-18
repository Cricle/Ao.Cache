using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ISyncWithDataFinder<TIdentity,TEntity> : ISyncDataFinder<TIdentity, TEntity>, ISyncWithDataAccesstorFinder<TIdentity, TEntity>
    {

    }
    public interface IWithDataFinder<TIdentity,TEntity> : IDataFinder<TIdentity, TEntity>,IWithDataAccesstorFinder<TIdentity,TEntity>
    {

    }
    public interface IDataFinder<TIdentity,TEntity> : IWithDataFinderOptions<TIdentity, TEntity>, ICacheFinder<TIdentity, TEntity>, IPhysicalFinder<TIdentity, TEntity>, IRenewalable<TIdentity>
    {
        Task<bool> DeleteAsync(TIdentity identity);

        Task<bool> ExistsAsync(TIdentity identity);
    }
    public interface ISyncDataFinder<TIdentity,TEntity> : IWithDataFinderOptions<TIdentity, TEntity>, ISyncCacheFinder<TIdentity, TEntity>, ISyncPhysicalFinder<TIdentity, TEntity>, ISyncRenewalable<TIdentity>
    {
        bool Delete(TIdentity identity);

        bool Exists(TIdentity identity);
    }
}
