using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IPhysicalFinder<TIdentity, TEntity>
    {
        Task<TEntity> FindInDbAsync(TIdentity identity, bool cache);
    }

}
