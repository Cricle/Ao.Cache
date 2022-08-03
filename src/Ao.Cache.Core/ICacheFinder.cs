using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ICacheFinder
    {
        Task<bool> SetInCahceAsync(object identity, object entity);

        Task<object> FindInCahceAsync(object identity);
    }
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity);

        Task<TEntity> FindInCahceAsync(TIdentity identity);
    }

}
