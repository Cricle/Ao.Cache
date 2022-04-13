using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity);

        Task<TEntity> FindInCahceAsync(TIdentity identity);

    }

}
