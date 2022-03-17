using System.Threading.Tasks;

namespace Ao.Cache.Redis.Finders
{
    public interface IWorkDataFinder<TIdentity, TEntry>
    {
        Task<TEntry> FindAsync(TIdentity identity);
    }

}