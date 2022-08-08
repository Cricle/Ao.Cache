using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultBitRedisDataFinder<TIdentity, TEntity>: BitRedisDataFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisDataFinder(IDatabase database,
            IDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }
        public IDatabase Database { get; }

        public IDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        public override IDatabase GetDatabase()
        {
            return Database;
        }

        protected override Task<TEntity> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }

    }
}
