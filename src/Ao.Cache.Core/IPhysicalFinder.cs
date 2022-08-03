using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IPhysicalFinder
    {
        Task<object> FindInDbAsync(object identity, bool cache);
    }
    public interface IPhysicalFinder<TIdentity, TEntity>
    {
        Task<TEntity> FindInDbAsync(TIdentity identity, bool cache);
    }

}
