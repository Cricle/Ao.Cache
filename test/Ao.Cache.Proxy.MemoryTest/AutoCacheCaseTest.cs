using Ao.Cache.CastleProxy.Interceptors;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Ao.Cache.Proxy.MemoryTest
{
    [TestClass]
    public class AutoCacheCaseTest : AutoTestBase<NowService>
    {
        public override void Config(IContainer container)
        {
            container.Register<NowService>();
            container.AsyncIntercept<NowService, CacheInterceptor>();
        }
        [TestMethod]
        public async Task LockCache()
        {
            var provider = CreateProvider(x =>
            {
                x.Register<LockCache>();
                x.AsyncIntercept<LockCache, CacheInterceptor>();
            });
            var gt = provider.GetRequiredService<LockCache>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();
            await finderFc.DeleteAsync<LockCache, DateTime?>(x => x.Now());//Clear up
            var tasks = new Task[10];
            var times = new ConcurrentBag<DateTime>();
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = await Task.Factory.StartNew(async () =>
                {
                    var d = await gt.Now();
                    times.Add(d.Value);
                });
            }
            await Task.WhenAll(tasks);
            var group = times.GroupBy(x => x).Count();
            Assert.AreEqual(1, group);
        }
    }
}
