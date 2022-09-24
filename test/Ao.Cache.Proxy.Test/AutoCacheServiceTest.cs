namespace Ao.Cache.Proxy.Test
{
    class NullDataFinder<TKey, TValue> : DataFinderBase<TKey, TValue>
    {
        public override Task<bool> DeleteAsync(TKey entity)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ExistsAsync(TKey identity)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RenewalAsync(TKey identity, TimeSpan? time)
        {
            throw new NotImplementedException();
        }

        protected override Task<TValue> CoreFindInCacheAsync(string key, TKey identity)
        {
            throw new NotImplementedException();
        }

        protected override Task<TValue> OnFindInDbAsync(TKey identity)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> SetInCacheAsync(string key, TKey identity, TValue entity, TimeSpan? caheTime)
        {
            throw new NotImplementedException();
        }
    }
    [TestClass]
    public class AutoCacheServiceTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            var moq = new Mock<IDataFinderFactory>();
            var factory = moq.Object;
            var namedHelper = DefaultCacheNamedHelper.Default;

            Assert.ThrowsException<ArgumentNullException>(() => new AutoCacheService(factory, null));
            Assert.ThrowsException<ArgumentNullException>(() => new AutoCacheService(null, namedHelper));
        }
        [TestMethod]
        public void GetEmpty()
        {
            var inst = new NullDataFinder<UnwindObject, object>();
            var moq = new Mock<IDataFinderFactory>();
            moq.Setup(x => x.Create(It.IsAny<IDataAccesstor<UnwindObject, object>>()))
                .Returns(inst);
            var factory = moq.Object;

            var ser = new AutoCacheService(factory, DefaultCacheNamedHelper.Default);

            Assert.AreEqual(DefaultCacheNamedHelper.Default, ser.NamedHelper);

            var act = ser.GetEmpty<object>();

            Assert.AreEqual(inst, act);

            act = ser.Get(EmptyDataAccesstor<UnwindObject, object>.Instance);

            Assert.AreEqual(inst, act);
        }
    }
}
