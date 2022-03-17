using Ao.ObjectDesign;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Finders
{
    public abstract class ColumnCacheFinder<TIdentity, TEntity,TValue> : IDataFinder<TIdentity, TEntity>,IDisposable
    {
        public static readonly TimeSpan DefaultCacheTime = TimeSpan.FromSeconds(3);

        protected static readonly bool IsNormalType = typeof(TEntity).IsPrimitive || typeof(TEntity) == typeof(string);
        protected static readonly bool IsArray = typeof(TEntity).IsArray;
        protected static readonly Type EntityType = typeof(TEntity);

        private ICacheOperator<TValue> @operator;
        private TypeCreator creator;

        public TypeCreator Creator => creator;

        public ICacheOperator<TValue> Operator => @operator;

        public void Build()
        {
            if (!IsNormalType)
            {
                creator = CompiledPropertyInfo.GetCreator(EntityType);
            }
            @operator = GetOperator();
            OnBuild();
        }
        protected virtual void OnBuild()
        {

        }

        protected abstract ICacheOperator<TValue> GetOperator();

        public virtual async Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            var data = await GetValueAsync(key,identity);
            if (data != null)
            {
                return Write(identity, data);
            }
            return default;
        }
        protected abstract TEntity Write(TIdentity identity,TValue value);
        protected abstract Task<TValue> GetValueAsync(string key,TIdentity identity);
        protected virtual TEntity Create()
        {
            if (IsNormalType)
            {
                return default;
            }
            if (IsArray)
            {
                throw new InvalidOperationException($"Can't operator raw array!");
            }
            return (TEntity)Creator();
        }
        protected virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }
        protected virtual string GetHead()
        {
            return GetType().FullName;
        }
        protected string GetEntryKey(TIdentity identity)
        {
            return string.Concat(GetHead(), ".", GetPart(identity));
        }
        public async Task<TEntity> FindInDbAsync(TIdentity identity, bool cache = true)
        {
            var entry = await OnFindInDbAsync(identity);
            if (entry != null && cache)
            {
                await SetInCahceAsync(identity, entry);
            }
            return entry;
        }

        protected abstract Task<object> CoreGetColumn(TIdentity identity, ICacheColumn column);

        protected virtual bool CheckColumn(TIdentity identity, ICacheColumn column)
        {
            return true;
        }
        protected abstract Task<TEntity> OnFindInDbAsync(TIdentity identity);

        public Task<object> GetColumnValueAsync(TIdentity identity,ICacheColumn column)
        {
            if (!CheckColumn(identity, column))
            {
                return null;
            }
            return CoreGetColumn(identity, column);
        }

        public Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity)
        {
            var key = GetEntryKey(identity);
            var h = @operator.As(entity);
            var cacheTime = GetCacheTime(identity, entity);
            return CoreSetInCacheAsync(identity, entity, key, h,cacheTime);
        }
        protected abstract Task<bool> CoreSetInCacheAsync(TIdentity identity, TEntity entity, string key, TValue value,TimeSpan? cacheTime);
        protected virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DefaultCacheTime;
        }

        public virtual void Dispose()
        {
        }
    }

}