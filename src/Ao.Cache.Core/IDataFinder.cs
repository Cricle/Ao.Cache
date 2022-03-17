using System.Text;

namespace Ao.Cache
{
    public interface IDataFinder<TIdentity, TEntry>: ICacheFinder<TIdentity,TEntry>, IPhysicalFinder<TIdentity,TEntry>
    {

    }

}
