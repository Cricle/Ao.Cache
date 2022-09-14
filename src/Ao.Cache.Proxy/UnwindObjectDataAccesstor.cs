using System;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public readonly struct CastleDataAccesstor<TKey, TEntity> : IDataAccesstor<TKey, TEntity>
    {
        public readonly Func<Task<TEntity>> Proceed;

        public CastleDataAccesstor(Func<Task<TEntity>> proceed)
        {
            Proceed = proceed;
        }

        public Task<TEntity> FindAsync(TKey identity)
        {
            return Proceed();
        }
    }
}
