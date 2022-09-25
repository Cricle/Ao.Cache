using Ao.Cache.Proxy.Annotations;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoCacheAssertionsTest
    {
        [ExcludeFromCodeCoverage]
        class A
        {
            [AutoCache]
            public void HasTag()
            {

            }
            public void NoTag()
            {

            }
        }
        [TestMethod]
        public void HasAutoCache()
        {
            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(AutoCacheAssertions.HasAutoCache(typeof(A).GetMethod(nameof(A.HasTag))));
            }
        }
        [TestMethod]
        public void NoAutoCache()
        {
            for (int i = 0; i < 2; i++)
            {
                Assert.IsFalse(AutoCacheAssertions.HasAutoCache(typeof(A).GetMethod(nameof(A.NoTag))));
            }
        }
    }
}
