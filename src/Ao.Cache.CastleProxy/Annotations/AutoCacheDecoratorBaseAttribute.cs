using Ao.Cache.CastleProxy.Model;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    public abstract class AutoCacheDecoratorBaseAttribute : Attribute
    {
        public int Order { get; set; }

        public virtual Task InterceptBeginAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context)
        {
            return TaskHelper.ComplatedTask;
        }
        public virtual Task InterceptFinallyAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context)
        {
            return TaskHelper.ComplatedTask;
        }
        public virtual Task InterceptEndAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context, AutoCacheInvokeResultContext<TResult> resultContext)
        {
            return TaskHelper.ComplatedTask;
        }
        public virtual Task InterceptExceptionAsync<TResult>(AutoCacheInvokeDecoratorContext<TResult> context,Exception exception)
        {
            return TaskHelper.ComplatedTask;
        }
        public virtual Task DecorateAsync<TResult>(AutoCacheDecoratorContext<TResult> context)
        {
            return TaskHelper.ComplatedTask;
        }

        public virtual Task FoundInCacheAsync<TResult>(AutoCacheDecoratorContext<TResult> context, TResult result)
        {
            return TaskHelper.ComplatedTask;
        }

        public virtual Task FoundInMethodAsync<TResult>(AutoCacheDecoratorContext<TResult> context, TResult result)
        {
            return TaskHelper.ComplatedTask;
        }
    }
}
