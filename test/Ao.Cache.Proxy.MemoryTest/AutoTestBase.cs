using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Ao.Cache.Proxy.MemoryTest
{
    public abstract class AutoTestProvider
    {

        protected virtual IServiceProvider CreateProvider(Action<IContainer> action)
        {
            var ser = new ServiceCollection();
            ser.WithCastleCacheProxy();
            ser.AddInMemoryFinder();

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            action?.Invoke(icon);
            return icon.BuildServiceProvider();

        }
    }
    public abstract class AutoTestBase<T>: AutoTestProvider
        where T: INowService
    {

        public abstract void Config(IContainer container);
       
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task WithArgumentSkip(bool cacheExpression)
        {
            var provider = CreateProvider(Config);


            var nowSer = provider.GetRequiredService<T>();
            var cs = provider.GetRequiredService<AutoCacheService>();

            var exists = await cs.ExistsAsync<T, DateTime?>(x => x.NowWithArg(1, "1"), cacheExpression);
            Assert.IsFalse(exists);

            var r1 = await nowSer.NowWithArg(1, "2");
            var r2 = await nowSer.NowWithArg(1, "3");

            Assert.AreEqual(r1, r2);

            var mem = provider.GetRequiredService<IMemoryCache>();
            exists = await cs.ExistsAsync<T, DateTime?>(x => x.NowWithArg(1, "4"), cacheExpression);
            Assert.IsTrue(exists);

            var r = await cs.RenewalAsync<T, DateTime?>(TimeSpan.FromSeconds(5), x => x.NowWithArg(1, "5"), cacheExpression);
            Assert.IsTrue(r);

            r = await cs.DeleteAsync<T, DateTime?>(x => x.NowWithArg(1, "6"), cacheExpression);
            Assert.IsTrue(r);

            exists = await cs.ExistsAsync<T, DateTime?>(x => x.NowWithArg(1, "8"), cacheExpression);
            Assert.IsFalse(exists);

            var r3 = await nowSer.NowWithArg(1, "7");
            Assert.AreNotEqual(r1, r3);

        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task FindAndInCache(bool cacheExpression)
        {
            var provider = CreateProvider(Config);

            var nowSer = provider.GetRequiredService<T>();
            var cs = provider.GetRequiredService<AutoCacheService>();

            var exists = await cs.ExistsAsync<T, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsFalse(exists);

            var r1 = await nowSer.Now();
            var r2 = await nowSer.Now();

            Assert.AreEqual(r1, r2);

            exists = await cs.ExistsAsync<T, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsTrue(exists);

            var r = await cs.RenewalAsync<T, DateTime?>(TimeSpan.FromSeconds(5), x => x.Now(), cacheExpression);
            Assert.IsTrue(r);

            r = await cs.DeleteAsync<T, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsTrue(r);

            exists = await cs.ExistsAsync<T, DateTime?>(x => x.Now(), cacheExpression);
            Assert.IsFalse(exists);

            var r3 = await nowSer.Now();

            Assert.AreNotEqual(r1, r3);


        }

    }
}
