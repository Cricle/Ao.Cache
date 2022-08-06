using System.Threading.Tasks;

namespace Ao.Cache
{
    public class EmptyDataFinderFactory<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>
    {
        private EmptyDataFinderFactory() { }

        public static readonly EmptyDataFinderFactory<TIdentity, TEntity> Instance = new EmptyDataFinderFactory<TIdentity, TEntity>();

        public Task<TEntity> FindAsync(TIdentity identity)
        {
            throw new System.NotImplementedException();
        }
    }
}
