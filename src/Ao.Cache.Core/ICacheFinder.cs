using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<bool> SetInCacheAsync(TIdentity identity, TEntity entity);

        Task<TEntity> FindInCacheAsync(TIdentity identity);
    }

}
