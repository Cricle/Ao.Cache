using Ao.Cache.CastleProxy.Events;
using Ao.Cache.CastleProxy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AutoCacheEventAttribute : AutoCacheDecoratorBaseAttribute
    {
        public static AutoCacheEventPublishTypes DefaultPublishType { get; set; } = AutoCacheEventPublishTypes.MethodFound;

        public string Key { get; set; }

        public AutoCacheEventPublishTypes PublishType { get; set; } = DefaultPublishType;

        protected virtual string GetChannelKey<T>()
        {
            return EventHelper.GetChannelKey<T>(Key);
        }

        public override Task FindInMethodEndAsync<TResult>(AutoCacheDecoratorContext<TResult> context, TResult result, bool isFromIntercept)
        {
            if ((PublishType & AutoCacheEventPublishTypes.MethodFound) != 0)
            {
                return PublishAsync(context.ServiceProvider, result, AutoCacheEventPublishTypes.MethodFound);
            }
            return base.FindInMethodEndAsync(context, result, isFromIntercept);
        }
        public override Task FoundInCacheAsync<TResult>(AutoCacheDecoratorContext<TResult> context, TResult result)
        {
            if ((PublishType & AutoCacheEventPublishTypes.CacheFound) != 0)
            {
                return PublishAsync(context.ServiceProvider, result, AutoCacheEventPublishTypes.CacheFound);
            }
            return base.FoundInCacheAsync(context, result);
        }
        private Task<EventPublishResult> PublishAsync<TResult>(IServiceProvider provider,TResult result, AutoCacheEventPublishTypes type)
        {
            var key = GetChannelKey<TResult>();
            var adapter = provider.GetService<IEventAdapter>();
            return adapter.PublishAsync(key, new AutoCacheEventPublishState<TResult>
            {
                Data = result,
                Type = type
            });
        }
    }
}
