using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ICacheFinder<TIdentity, TEntry>
    {
        Task<bool> SetInCahceAsync(TIdentity identity, TEntry entity);

        Task<TEntry> FindInCahceAsync(TIdentity identity);

    }

}
