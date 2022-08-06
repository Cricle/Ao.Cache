using System.Threading.Tasks;

namespace Ao.Cache
{
    public class EmptyDataFinderFactory<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>
    {
        public Task<TEntity> FindAsync(TIdentity identity)
        {
            throw new System.NotImplementedException();
        }
    }
}
