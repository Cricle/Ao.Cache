using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Finders
{
    public abstract class ListCacheFinder<TIdentity, TEntry> : ColumnCacheFinder<TIdentity, TEntry,RedisValue[]>
    {
        private ExpressionListCacheOperator expressionCacher;

        public ExpressionListCacheOperator ExpressionCacher => expressionCacher;

        public abstract IDatabase GetDatabase();

        protected override ICacheOperator<RedisValue[]> GetOperator()
        {
            if (EntityType.IsPrimitive ||
                EntityType == typeof(string) ||
                Nullable.GetUnderlyingType(EntityType) != null)
            {
                return RawCacheOperator.GetRedisOperator(EntityType);
            }
            return ExpressionListCacheOperator.GetRedisOperator(EntityType);
        }
        protected override void OnBuild()
        {
            expressionCacher = Operator as ExpressionListCacheOperator;

        }

        protected override TEntry Write(TIdentity identity, RedisValue[] value)
        {
            if (value.Length==0)
            {
                return default;
            }
            if (expressionCacher != null)
            {
                return (TEntry)expressionCacher.Write(value);
            }
            object inst = Create();
            Operator.Write(ref inst, value);
            return (TEntry)inst;
        }

        protected override Task<RedisValue[]> GetValueAsync(string key, TIdentity identity)
        {
            return GetDatabase().ListRangeAsync(key);
        }

        protected override async Task<object> CoreGetColumn(TIdentity identity, ICacheColumn column)
        {
            var index = GetIndex(column);
            if (index==-1)
            {
                return null;
            }
            var val = await GetDatabase().ListGetByIndexAsync(GetEntryKey(identity), index);
            if (val.HasValue)
            {
                return null;
            }
            return column.Converter == null ? val : column.Converter.ConvertBack(val, column);
        }
        private int GetIndex(ICacheColumn column)
        {
            var allColumns = expressionCacher.AllColumns;
            var len = allColumns.Count;
            for (int i = 0; i < len; i++)
            {
                if (allColumns[i]==column)
                {
                    return i;
                }
            }
            return -1;
        }
        protected override string GetHead()
        {
            return "List." + base.GetHead();
        }
        protected override bool CheckColumn(TIdentity identity, ICacheColumn column)
        {
            return expressionCacher != null;
        }

        protected override async Task<bool> CoreSetInCacheAsync(TIdentity identity, TEntry entity, string key, RedisValue[] value, TimeSpan? cacheTime)
        {
            var db = GetDatabase();
            await db.ListRightPushAsync(key, value);
            await db.KeyExpireAsync(key, cacheTime);
            return true;
        }
    }

}