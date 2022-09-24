using Ao.Cache.Proxy.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class StringTransferAttributeTest
    {
        [TestMethod]
        public void GivenNullOrNotImpl_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StringTransferAttribute(null));
            Assert.ThrowsException<ArgumentException>(() => new StringTransferAttribute(typeof(int)));
        }
        [TestMethod]
        public void Given_PropertyMustEqualsInput()
        {
            var attr = new StringTransferAttribute(typeof(NullStringTransfer));
            Assert.AreEqual(typeof(NullStringTransfer), attr.StringTransferType);
        }
    }
}
