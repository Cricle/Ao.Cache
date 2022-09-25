using Ao.Cache.Proxy.Model;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class CacheResultNewExpressionTest
    {
        [TestMethod]
        public void IsAutoCacheResult()
        {
            Assert.AreEqual(typeof(AutoCacheResult<object>), CacheResultNewExpression<AutoCacheResult<object>>.Type);
            Assert.IsTrue(CacheResultNewExpression<AutoCacheResult<object>>.IsAutoResult);
            Assert.AreEqual(typeof(object), CacheResultNewExpression<AutoCacheResult<object>>.GenericType);
            Assert.IsInstanceOfType(CacheResultNewExpression<AutoCacheResult<object>>.Creator(), typeof(AutoCacheResult<object>));
        }
        [TestMethod]
        public void IsNotAutoCacheResult()
        {
            Assert.AreEqual(typeof(object), CacheResultNewExpression<object>.Type);
            Assert.IsFalse(CacheResultNewExpression<object>.IsAutoResult);
            Assert.IsNull(CacheResultNewExpression<object>.GenericType);
            Assert.IsInstanceOfType(CacheResultNewExpression<object>.Creator(), typeof(object));
        }
    }
}
