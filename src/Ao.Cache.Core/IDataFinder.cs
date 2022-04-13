using System.Text;

namespace Ao.Cache
{
    public interface IDataFinder<TIdentity, TEntity>: ICacheFinder<TIdentity,TEntity>, IPhysicalFinder<TIdentity,TEntity>
    {

    }

}
