using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public readonly struct FuncDataAccesstor<TKey, TEntity> : IDataAccesstor<TKey, TEntity>
    {
        public readonly Func<Task<TEntity>> Proceed;

        public FuncDataAccesstor(Func<Task<TEntity>> proceed)
        {
            Proceed = proceed ?? throw new ArgumentNullException(nameof(proceed));
        }

        public Task<TEntity> FindAsync(TKey identity)
        {
            return Proceed();
        }
    }
}
