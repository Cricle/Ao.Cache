using Ao.Cache.Proxy.Annotations;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoCacheResultBoxTest
    {
        [TestMethod]
        public void EmptyState()
        {
            var box = new AutoCacheResultBox<object>();

            Assert.IsNull(box.Result);
            Assert.IsFalse(box.HasResult);
        }
        [TestMethod]
        public void HasResult()
        {
            var box = new AutoCacheResultBox<object>();
            box.SetResult(1);

            Assert.AreEqual(1, box.Result);
            Assert.IsTrue(box.HasResult);
        }
    }
}
