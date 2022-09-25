using Ao.Cache.Events;
using Ao.Cache.Proxy.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoCacheEventAttributeTest
    {
        [ExcludeFromCodeCoverage]
        class NullEventAdapter : IEventAdapter
        {
            public Task<EventPublishResult> PublishAsync<T>(string channel, T data)
            {
                return Task.FromResult(new EventPublishResult(true));
            }

            public Task<IDisposable> SubscribeAsync<T>(string channel, IEventReceiver<T> receiver)
            {
                return Task.FromResult<IDisposable>(null);
            }
        }
        [TestMethod]
        public void Defaults()
        {
            var attr = new AutoCacheEventAttribute();
            Assert.AreEqual(AutoCacheEventAttribute.DefaultPublishType, attr.PublishType);
        }
        [TestMethod]
        public async void Publish()
        {
            var sers = new ServiceCollection();
            sers.AddSingleton<IEventAdapter, NullEventAdapter>();
            var provider = sers.BuildServiceProvider();
            var attr = new AutoCacheEventAttribute
            {
                PublishType = AutoCacheEventPublishTypes.CacheFound | AutoCacheEventPublishTypes.MethodFound
            };

            var tsk = attr.FindInMethodEndAsync<object>(
                new AutoCacheDecoratorContext<object>(null, provider, null, default),
                null,
                false);
            await tsk;
            Assert.IsInstanceOfType(tsk, typeof(Task<EventPublishResult>));

            tsk = attr.FoundInCacheAsync<object>(
                new AutoCacheDecoratorContext<object>(null, provider, null, default),
                null);
            await tsk;
            Assert.IsInstanceOfType(tsk, typeof(Task<EventPublishResult>));
        }
    }
}
