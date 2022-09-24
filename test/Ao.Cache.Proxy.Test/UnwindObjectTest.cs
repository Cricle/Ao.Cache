using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class UnwindObjectTest
    {
        [TestMethod]
        public void GivenNull_MustThrowExcepton()
        {
            var header = "123";
            var arr = new object[] { 1 };
            var st = DefaultStringTransfer.Default;

            Assert.ThrowsException<ArgumentNullException>(() => new UnwindObject(header, arr, null));
            Assert.ThrowsException<ArgumentNullException>(() => new UnwindObject(header, null, st));
            Assert.ThrowsException<ArgumentNullException>(() => new UnwindObject(null, arr, st));
        }
        [TestMethod]
        public void HashCodeAndEquals()
        {
            var header = "123";
            var header1 = "123111";
            var arr = new object[] { 1 };
            var st = DefaultStringTransfer.Default;

            var u1 = new UnwindObject(header, arr, st);
            var u2 = new UnwindObject(header1, arr, st);

            Assert.AreEqual(u1.GetHashCode(), u1.GetHashCode());
            Assert.AreNotEqual(u1.GetHashCode(), u2.GetHashCode());

            Assert.IsTrue(u1.Equals(u1));
            Assert.IsTrue(u1.Equals((object)u1));

            Assert.IsFalse(u1.Equals((object)u2));
            Assert.IsFalse(u1.Equals(null));
            Assert.IsFalse(u1.Equals(default(UnwindObject)));

        }
        [TestMethod]
        public void MakeString()
        {
            var header = "123";
            var arr = new object[] { 1 };
            var st = DefaultStringTransfer.Default;

            var u1 = new UnwindObject(header, arr, st);


            Assert.AreEqual(st, u1.ObjectTransfer);

            Assert.AreEqual($"123{DefaultStringTransfer.DefaultSpliter}1", u1.ToString());
        }
    }
}
