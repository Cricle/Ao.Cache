using Ao.Cache.Proxy.Annotations;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoLockAttributeTest
    {
        [TestMethod]
        public void Defaults()
        {
            var attr = new AutoLockAttribute();

            Assert.AreEqual(AutoLockAttribute.DefaultExpireTime, attr.ExpireTime);
        }
        [TestMethod]
        public void Parse()
        {
            var attr = new AutoLockAttribute("00:00:05");

            Assert.AreEqual(TimeSpan.FromSeconds(5), attr.ExpireTime);
        }
        [TestMethod]
        public void Input()
        {
            var attr = new AutoLockAttribute(TimeSpan.FromSeconds(5));

            Assert.AreEqual(TimeSpan.FromSeconds(5), attr.ExpireTime);
        }
    }
}
