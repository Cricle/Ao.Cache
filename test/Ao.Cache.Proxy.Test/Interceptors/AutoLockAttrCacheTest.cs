using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;

namespace Ao.Cache.Proxy.Test.Interceptors
{
    [TestClass]
    public class AutoLockAttrCacheTest
    {
        [ExcludeFromCodeCoverage]
        class A
        {
            [AutoLock]
            public void HasTag()
            {

            }
            public void NoTag()
            {

            }
        }
        [AutoLock]
        [ExcludeFromCodeCoverage]
        class B
        {
            public void NoTag()
            {

            }
        }
        [TestMethod]
        public void HasLock()
        {
            for (int i = 0; i < 2; i++)
            {
                Assert.IsNotNull(AutoLockAttrCache.Get(new NamedInterceptorKey(typeof(A), typeof(A).GetMethod(nameof(A.HasTag)))));
                Assert.IsNotNull(AutoLockAttrCache.Get(new NamedInterceptorKey(typeof(B), typeof(B).GetMethod(nameof(B.NoTag)))));
            }
        }
        [TestMethod]
        public void NoLock()
        {
            for (int i = 0; i < 2; i++)
            {
                Assert.IsNull(AutoLockAttrCache.Get(new NamedInterceptorKey(typeof(A), typeof(A).GetMethod(nameof(A.NoTag)))));
            }
        }
    }
}
