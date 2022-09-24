using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class DefaultStringTransferTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DefaultStringTransfer(null));
        }
        [TestMethod]
        public void ToStringObject()
        {
            Assert.AreEqual(string.Empty, DefaultStringTransfer.Default.ToString(null));
            Assert.AreEqual("aaa", DefaultStringTransfer.Default.ToString("aaa"));
            Assert.AreEqual("1", DefaultStringTransfer.Default.ToString(1));
            Assert.AreEqual("123.456", DefaultStringTransfer.Default.ToString(123.456m));
        }
        [TestMethod]
        public void Combines()
        {
            Assert.AreEqual("aaa", DefaultStringTransfer.Default.Combine("aaa"));
            Assert.AreEqual($"aaa{DefaultStringTransfer.DefaultSpliter}{1}", DefaultStringTransfer.Default.Combine("aaa", new object[] { "1" }));
            Assert.AreEqual($"aaa{DefaultStringTransfer.DefaultSpliter}{1}{DefaultStringTransfer.DefaultSpliter}{2}", DefaultStringTransfer.Default.Combine("aaa", new object[] { "1" ,"2"}));
        }
    }
}
