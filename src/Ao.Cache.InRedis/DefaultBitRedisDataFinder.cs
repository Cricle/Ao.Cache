using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultBitRedisDataFinder<TIdentity, TEntity>: BitRedisDataFinder<TIdentity, TEntity>,IWithDataAccesstorFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisDataFinder(IDatabase database,
            IDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(database,entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        protected override Task<TEntity> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }

    }
}
