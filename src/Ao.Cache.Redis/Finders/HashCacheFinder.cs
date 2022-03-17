using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Finders
{
    public abstract class HashCacheFinder<TIdentity, TEntity> : ColumnCacheFinder<TIdentity, TEntity, HashEntry[]>
    {
        private ExpressionHashCacheOperator expressionCacher;

        public ExpressionHashCacheOperator ExpressionCacher => expressionCacher;

        public IDatabase Database { get; }

        protected HashCacheFinder(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }
        protected override ICacheOperator<HashEntry[]> GetOperator()
        {
            if (EntityType.IsPrimitive ||
                EntityType == typeof(string) ||
                Nullable.GetUnderlyingType(EntityType) != null)
            {
                return RawCacheOperator.GetRedisOperator(EntityType);
            }
            return ExpressionHashCacheOperator.GetRedisOperator(EntityType);
        }
        protected override void OnBuild()
        {
            expressionCacher = Operator as ExpressionHashCacheOperator;

        }

        protected override TEntity Write(TIdentity identity, HashEntry[] value)
        {
            if (value.Length == 0)
            {
                return default;
            }
            if (expressionCacher != null)
            {
                return (TEntity)expressionCacher.Write(value);
            }
            object inst = Create();
            Operator.Write(ref inst, value);
            return (TEntity)inst;
        }

        protected override Task<HashEntry[]> GetValueAsync(string key, TIdentity identity)
        {
            return Database.HashGetAllAsync(key);
        }

        protected override async Task<object> CoreGetColumn(TIdentity identity, ICacheColumn column)
        {
            var val = await Database.HashGetAsync(GetEntryKey(identity), column.Path);
            if (val.HasValue)
            {
                return null;
            }
            return column.Converter == null ? val : column.Converter.ConvertBack(val, column);
        }
        protected override bool CheckColumn(TIdentity identity, ICacheColumn column)
        {
            return expressionCacher != null;
        }

        protected override async Task<bool> CoreSetInCacheAsync(TIdentity identity, TEntity entity, string key, HashEntry[] value, TimeSpan? cacheTime)
        {
            await Database.HashSetAsync(key, value);
            await Database.KeyExpireAsync(key, cacheTime);
            return true;
        }
    }

}