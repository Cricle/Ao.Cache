using System.Security.Principal;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IPhysicalFinder<TIdentity, TEntity>
    {
        Task<TEntity> FindInDbAsync(IDataAccesstor<TIdentity,TEntity> dataAccesstor,TIdentity identity, bool cache);
    }
    public interface ISyncPhysicalFinder<TIdentity, TEntity>
    {
        TEntity FindInDb(ISyncDataAccesstor<TIdentity, TEntity> dataAccesstor, TIdentity identity, bool cache);
    }
}
