using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IPhysicalFinder<TIdentity, TEntry>
    {
        Task<TEntry> FindInDbAsync(TIdentity identity, bool cache);
    }

}
