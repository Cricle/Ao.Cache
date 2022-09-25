using Ao.Cache.Proxy.Interceptors;

namespace Ao.Cache.Proxy.Test.Interceptors
{
    [TestClass]
    public class NamedInterceptorKeyTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            var t = typeof(object);
            var m = t.GetMethods()[0];

            Assert.ThrowsException<ArgumentNullException>(() => new NamedInterceptorKey(t, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NamedInterceptorKey(null, m));
        }
        [TestMethod]
        public void HashCodeEqualsAndString()
        {
            var t = typeof(object);
            var m1 = t.GetMethods()[0];
            var m2 = t.GetMethods()[1];

            var a = new NamedInterceptorKey(t, m1);
            var b = new NamedInterceptorKey(t, m2);

            Assert.AreEqual(t, a.TargetType);
            Assert.AreEqual(m1, a.Method);

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

            Assert.AreEqual(a.ToString(), a.ToString());
            Assert.AreNotEqual(a.ToString(), b.ToString());

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object)a));

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals((object)b));
            Assert.IsFalse(a.Equals(default));
        }
    }
}
