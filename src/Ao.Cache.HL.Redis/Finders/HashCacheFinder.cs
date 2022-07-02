using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.HL.Redis.Finders
{
    public abstract class HashCacheFinder<TIdentity, TEntity> : ColumnCacheFinder<TIdentity, TEntity, HashEntry[]>
    {
        private ExpressionHashCacheOperator expressionCacher;

        public ExpressionHashCacheOperator ExpressionCacher => expressionCacher;

        public abstract IDatabase GetDatabase();

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
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return GetDatabase().KeyExpireAsync(GetEntryKey(identity), time);
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
            return GetDatabase().HashGetAllAsync(key);
        }

        protected override async Task<object> CoreGetColumn(TIdentity identity, ICacheColumn column)
        {
            var val = await GetDatabase().HashGetAsync(GetEntryKey(identity), column.Path);
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
        protected override string GetHead()
        {
            return "Hash." + base.GetHead();
        }

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            return GetDatabase().KeyDeleteAsync(GetEntryKey(identity));
        }
        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            return GetDatabase().KeyExistsAsync(GetEntryKey(identity));
        }

        protected override async Task<bool> CoreSetInCacheAsync(TIdentity identity, TEntity entity, string key, HashEntry[] value, TimeSpan? cacheTime)
        {
            var db = GetDatabase();
            await db.HashSetAsync(key, value);
            await db.KeyExpireAsync(key, cacheTime);
            return true;
        }
    }

}