using Ao.Cache.Proxy.Interceptors;

namespace Ao.Cache.Proxy.Test.Interceptors
{
    [ExcludeFromCodeCoverage]
    class NullLocker : ILocker
    {
        string ILocker.Resource => throw new NotImplementedException();

        string ILocker.LockId => throw new NotImplementedException();

        bool ILocker.IsAcquired => throw new NotImplementedException();

        DateTime ILocker.CreateTime => throw new NotImplementedException();

        TimeSpan ILocker.ExpireTime => throw new NotImplementedException();

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
    [TestClass]
    public class RunLockResultTest
    {
        [TestMethod]
        public void Fail()
        {
            var res = new RunLockResult(RunLockResultTypes.SkipNoLocker);
            Assert.IsNull(res.Locker);
            Assert.AreEqual(RunLockResultTypes.SkipNoLocker, res.Type);
        }
        [TestMethod]
        public void WithLockers()
        {
            var locker = new NullLocker();
            var res = new RunLockResult(locker, RunLockResultTypes.InLocker);
            Assert.AreEqual(locker, res.Locker);
            Assert.AreEqual(RunLockResultTypes.InLocker, res.Type);
        }
    }
}
