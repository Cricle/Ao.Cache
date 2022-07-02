using Ao.ObjectDesign;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis.HashList
{
    public abstract class ComplexCacheOperator
    {
        public static readonly CacheColumnAnalysis SharedAnalysis = new CacheColumnAnalysis();

        protected ComplexCacheOperator(Type target, ICacheColumnAnalysis columnAnalysis)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ColumnAnalysis = columnAnalysis ?? throw new ArgumentNullException(nameof(columnAnalysis));
        }

        private IReadOnlyList<ICacheColumn> columns;
        private IReadOnlyDictionary<string, ICacheColumn> redisColumnMap;
        private TypeCreator creator;
        private ICacheColumn[] allColumns;
        private ICacheColumn[] noNextsColumns;
        private IReadOnlyDictionary<string, ICacheColumn> pathColumnMap;

        public Type Target { get; }

        public ICacheColumnAnalysis ColumnAnalysis { get; }

        public IReadOnlyList<ICacheColumn> Columns => columns;

        public IReadOnlyList<ICacheColumn> AllColumns => allColumns;

        public IReadOnlyList<ICacheColumn> NoNextAllColumns => noNextsColumns;

        public IReadOnlyDictionary<string, ICacheColumn> PathColumnMap => pathColumnMap;

        public IReadOnlyDictionary<string, ICacheColumn> RedisColumnMap => redisColumnMap;

        public void Build()
        {
            columns = BuildColumns();
            redisColumnMap = BuildColumnMap();
            if (!Target.IsPrimitive && Target != typeof(string)&&Nullable.GetUnderlyingType(Target)==null)
            {
                creator = CompiledPropertyInfo.GetCreator(Target);
            }
            var cls = new List<ICacheColumn>();
            IEnumerable<ICacheColumn> current = Columns;
            while (current.Any())
            {
                cls.AddRange(current);
                current = current.Where(x => x.Nexts != null).SelectMany(x => x.Nexts);
            }
            allColumns = cls.ToArray();
            noNextsColumns = allColumns.Where(x => x.Nexts == null || x.Nexts.Count == 0).ToArray();
            pathColumnMap = allColumns.ToDictionary(x => x.Path);

            OnBuild();
        }

        protected virtual void OnBuild()
        {

        }
        protected virtual IReadOnlyList<ICacheColumn> BuildColumns()
        {
            return ColumnAnalysis.GetRedisColumns(Target, null);
        }
        protected virtual IReadOnlyDictionary<string, ICacheColumn> BuildColumnMap()
        {
            return ColumnAnalysis.GetRedisColumnMap(Target, null);
        }

        public object Create()
        {
            return creator?.Invoke() ?? null;
        }
    }
}
