using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IWithDataAccesstorFinder<TIdentity, TEntity>
    {
        IDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        Task<TEntity> FindInDbAsync(TIdentity identity, bool cache);
    }
}
