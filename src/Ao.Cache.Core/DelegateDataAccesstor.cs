using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class DelegateDataAccesstor<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>
    {
        private readonly Func<TIdentity, Task<TEntity>> func;

        public DelegateDataAccesstor(Func<TIdentity, Task<TEntity>> func)
        {
            this.func = func;
        }

        public Task<TEntity> FindAsync(TIdentity identity)
        {
            return func(identity);
        }
    }
}
