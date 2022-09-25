using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class EmptyDataAccesstorTest
    {
        [TestMethod]
        public void AllMustThrowException()
        {
            Assert.ThrowsException<NotImplementedException>(() => EmptyDataAccesstor<object, object>.Instance.FindAsync((object)null));
            Assert.ThrowsException<NotImplementedException>(() => EmptyDataAccesstor<object, object>.Instance.FindAsync(null));
        }
    }
}
