using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IDataFinder<TIdentity, TEntity>: ICacheFinder<TIdentity,TEntity>, IPhysicalFinder<TIdentity,TEntity>
    {
        Task<bool> DeleteAsync(TIdentity identity);

        Task<bool> ExistsAsync(TIdentity identity);
    }

}
