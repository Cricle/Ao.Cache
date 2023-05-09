using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<bool> SetInCacheAsync(TIdentity identity, TEntity entity);

        Task<TEntity> FindInCacheAsync(TIdentity identity);
    }
    public interface ISyncCacheFinder<TIdentity, TEntity>
    {
        bool SetInCache(TIdentity identity, TEntity entity);

        TEntity FindInCache(TIdentity identity);
    }
}
