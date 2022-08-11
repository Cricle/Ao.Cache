using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    public abstract class AutoCacheDecoratorBaseAttribute : Attribute
    {
        public int Order { get; set; }

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
