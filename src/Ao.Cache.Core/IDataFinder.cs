using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IDataFinder : ICacheFinder, IPhysicalFinder,IRenewalable
    {
        Task<bool> DeleteAsync(object identity);

        Task<bool> ExistsAsync(object identity);
    }
    public interface IDataFinder<TIdentity, TEntity>: ICacheFinder<TIdentity,TEntity>, IPhysicalFinder<TIdentity,TEntity>,IRenewalable<TIdentity>
    {
        Task<bool> DeleteAsync(TIdentity identity);

        Task<bool> ExistsAsync(TIdentity identity);
    }

}
