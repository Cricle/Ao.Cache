using Ao.Cache.CastleProxy.Interceptors;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AutoCacheAttribute : AutoCacheDecoratorBaseAttribute
    {
        public AutoCacheAttribute()
        {
            Order = int.MinValue;
        }

        public override Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            var opt = IgnoreHeadDataFinderOptions<TResult>.Options;
            context.DataFinder.Options = opt;
            return base.DecorateAsync(context);
        }
    }
}
