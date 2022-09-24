using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class ActualTypeInfosTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            var t = typeof(object);

            Assert.ThrowsException<ArgumentNullException>(() => new ActualTypeInfos(t, null, false));
            Assert.ThrowsException<ArgumentNullException>(() => new ActualTypeInfos(null, t, false));
        }
        [TestMethod]
        public void HashCodeAndEquals()
        {
            var t1 = typeof(object);
            var t2 = typeof(double);

            var a1 = new ActualTypeInfos(t1, t2, false);
            var a2 = new ActualTypeInfos(t1, t1, false);
            var a3 = new ActualTypeInfos(t2, t2, false);

            Assert.AreEqual(a1.GetHashCode(), a1.GetHashCode());
            Assert.AreNotEqual(a1.GetHashCode(), a2.GetHashCode());
            Assert.AreNotEqual(a1.GetHashCode(), a3.GetHashCode());

            Assert.IsFalse(a1.Equals(a2));
            Assert.IsFalse(a1.Equals(a3));
            Assert.IsTrue(a1.Equals(a1));
        }
    }
}
