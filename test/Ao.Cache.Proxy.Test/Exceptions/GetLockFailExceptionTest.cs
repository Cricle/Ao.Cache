using Ao.Cache.Proxy.Exceptions;
using Ao.Cache.Proxy.Test.Interceptors;

namespace Ao.Cache.Proxy.Test.Exceptions
{
    [TestClass]
    public class GetLockFailExceptionTest
    {
        [TestMethod]
        public void Init()
        {
            var ex = new GetLockFailException();
            ex = new GetLockFailException("aaa");
            Assert.AreEqual("aaa", ex.Message);

            var innerEx = new Exception();
            ex = new GetLockFailException("aaa", innerEx);
            Assert.AreEqual("aaa", ex.Message);
            Assert.AreEqual(innerEx, ex.InnerException);

            var locker = new NullLocker();
            ex = new GetLockFailException { Locker = locker };
            ex.Locker = locker;
            Assert.AreEqual(locker, ex.Locker);
        }
    }
}
