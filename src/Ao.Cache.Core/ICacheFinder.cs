using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ICacheFinder
    {
        Task<bool> SetInCacheAsync(object identity, object entity);

        Task<object> FindInCacheAsync(object identity);
    }
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<bool> SetInCacheAsync(TIdentity identity, TEntity entity);

        Task<TEntity> FindInCacheAsync(TIdentity identity);
    }

}
