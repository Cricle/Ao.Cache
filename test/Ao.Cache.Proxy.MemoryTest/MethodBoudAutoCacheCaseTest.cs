using Ao.Cache.MethodBoundaryAspect.Interceptors;
using Ao.Cache.Proxy.Annotations;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.MemoryTest
{
    [TestClass]
    public class MethodBoudAutoCacheCaseTest : AutoTestBase<BoundNowService>
    {
        public override void Config(IContainer container)
        {
            container.Register<BoundNowService>();
        }
        protected override IServiceProvider CreateProvider(Action<IContainer> action)
        {
            var provider= base.CreateProvider(action);
            provider.SetGlobalMethodBoundaryFactory();
            return provider;
        }
        [TestMethod]
        public async Task LockCache()
        {
            var provider = CreateProvider(x =>
            {
                x.Register<BoundLockCache>();
            });
            var gt = provider.GetRequiredService<BoundLockCache>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();
            await finderFc.DeleteAsync<BoundLockCache, DateTime?>(x => x.Now());//Clear up
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
    [CacheInterceptor]
    public class BoundLockCache
    {
        [AutoCache]
        [AutoCacheOptions("00:01:00", Lock = true)]
        public async Task<DateTime?> Now()
        {
            await Task.Yield();
            Console.WriteLine("命中方法啦！");
            return DateTime.Now;
        }
    }
    [CacheInterceptor]
    public class BoundNowService : INowService
    {
        [AutoCache]
        [AutoCacheOptions("00:10:00")]
        public Task<DateTime?> Now()
        {
            return Task.FromResult<DateTime?>(DateTime.Now);
        }

        [AutoCache]
        public Task<DateTime?> NowWithArg(int a, [AutoCacheSkipPart]string b)
        {
            return Task.FromResult<DateTime?>(DateTime.Now);
        }
    }
}
