using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IWithDataAccesstorFinder<TIdentity, TEntity>
    {
        IDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        Task<TEntity> FindInDbAsync(TIdentity identity, bool cache);
    }
    public interface ISyncWithDataAccesstorFinder<TIdentity, TEntity>
    {
        IDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        TEntity FindInDb(TIdentity identity, bool cache);
    }
}
