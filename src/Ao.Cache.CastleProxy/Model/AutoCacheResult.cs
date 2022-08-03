using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Model
{
    public interface IAutoCacheResult
    {
        AutoCacheStatus Status { get; set; }
    }
    public static class ResultCreator
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<Type, Func<object>> creators = new Dictionary<Type, Func<object>>();

        public static object Create(Type resultType)
        {
            if (!creators.TryGetValue(resultType,out var f))
            {
                lock (locker)
                {
                    if (!creators.TryGetValue(resultType,out f))
                    {
                        f = Expression.Lambda<Func<object>>(Expression.New(typeof(AutoCacheResult<>).MakeGenericType(resultType)))
                            .Compile();
                        creators[resultType] = f;
                    }
                }
            }
            return f();
        }
    }
    public sealed class AutoCacheResult<T> : IAutoCacheResult
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status {get; set; }
    }
    [Flags]
    public enum AutoCacheStatus
    {
        Skip = 0,
        MethodHit = 1,
        CacheHit = 2,
        NotSupportFinderOrAccesstor = 3,
        Unknow = 4,
    }
}
