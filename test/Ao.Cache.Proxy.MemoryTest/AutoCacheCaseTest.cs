using Ao.Cache.CastleProxy;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.Events;
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
    public class AutoCacheCaseTest : AutoTestBase
    {
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
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task WithArgumentSkip(bool cacheExpression)
        {
            var provider = CreateProvider(x =>
            {
                x.Register<NowService>();
                x.AsyncIntercept<NowService, CacheInterceptor>();
            });


            var nowSer = provider.GetRequiredService<NowService>();
            var cs = provider.GetRequiredService<AutoCacheService>();

            var exists = await cs.ExistsAsync<NowService, DateTime?>(x => x.NowWithArg(1,"1"), cacheExpression);
            Assert.IsFalse(exists);

            var r1 = await nowSer.NowWithArg(1, "2");
            var r2 = await nowSer.NowWithArg(1, "3");

            Assert.AreEqual(r1, r2);

            exists = await cs.ExistsAsync<NowService, DateTime?>(x => x.NowWithArg(1, "4"), cacheExpression);
            Assert.IsTrue(exists);

            var r = await cs.RenewalAsync<NowService, DateTime?>(TimeSpan.FromSeconds(5), x => x.NowWithArg(1, "5"), cacheExpression);
            Assert.IsTrue(r);

            r = await cs.DeleteAsync<NowService, DateTime?>(x => x.NowWithArg(1, "6"), cacheExpression);
            Assert.IsTrue(r);

            exists = await cs.ExistsAsync<NowService, DateTime?>(x => x.NowWithArg(1, "8"), cacheExpression);
            Assert.IsFalse(exists);

            var r3 = await nowSer.NowWithArg(1, "7");
            Assert.AreNotEqual(r1, r3);

        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task FindAndInCache(bool cacheExpression)
        {
            var provider = CreateProvider(x =>
            {
                x.Register<NowService>();
                x.AsyncIntercept<NowService, CacheInterceptor>();
            });

            var nowSer = provider.GetRequiredService<NowService>();
            var cs = provider.GetRequiredService<AutoCacheService>();

            var exists = await cs.ExistsAsync<NowService, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsFalse(exists);

            var r1 = await nowSer.Now();
            var r2 = await nowSer.Now();

            Assert.AreEqual(r1, r2);

            exists = await cs.ExistsAsync<NowService, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsTrue(exists);

            var r = await cs.RenewalAsync<NowService, DateTime?>(TimeSpan.FromSeconds(5), x => x.Now(), cacheExpression);
            Assert.IsTrue(r);

            r = await cs.DeleteAsync<NowService, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsTrue(r);

            exists = await cs.ExistsAsync<NowService, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsFalse(exists);

            var r3 = await nowSer.Now();

            Assert.AreNotEqual(r1, r3);


        }
    }
}
