using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public readonly struct DelegateDataAccesstor<TIdentity, TEntity> : IDataAccesstor<TIdentity, TEntity>
    {
        private readonly Func<TIdentity, Task<TEntity>> func;
#if NET6_0||NETSTANDARD2_1
        public DelegateDataAccesstor(Func<TIdentity, ValueTask<TEntity>> func)
        {
            this.func = async identity => await func(identity);
        }
#endif
        public DelegateDataAccesstor(Func<TIdentity, TEntity> func)
        {
            this.func = identity=>Task.FromResult(func(identity));
        }
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
