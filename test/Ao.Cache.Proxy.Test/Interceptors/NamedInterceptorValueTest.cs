using Ao.Cache.Proxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test.Interceptors
{
    [TestClass]
    public class NamedInterceptorValueTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            var idx = new int[0];
            var trans = DefaultStringTransfer.Default;
            var h = "123";

            Assert.ThrowsException<ArgumentNullException>(() => new NamedInterceptorValue(idx, trans, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NamedInterceptorValue(idx, null, h));
        }


        [TestMethod]
        public void HashCodeEqualsAndString()
        {
            var idx = new int[0];
            var trans = DefaultStringTransfer.Default;
            var h = "123";
            var h1 = "1231";

            var a = new NamedInterceptorValue(idx, trans,h);
            var b = new NamedInterceptorValue(idx, trans, h1);

            Assert.AreEqual(h, a.Header);
            Assert.AreEqual(idx, a.ArgIndexs);
            Assert.AreEqual(trans, a.StringTransfer);

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

            Assert.AreEqual(a.ToString(), a.ToString());
            Assert.AreNotEqual(a.ToString(), b.ToString());

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object)a));

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals((object)b));
            Assert.IsFalse(a.Equals(default(NamedInterceptorValue)));
        }
    }
}
