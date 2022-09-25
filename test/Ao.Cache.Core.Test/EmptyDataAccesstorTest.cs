using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class EmptyDataAccesstorTest
    {
        [TestMethod]
        public void AllMustThrowException()
        {
            Assert.ThrowsException<NotImplementedException>(() => EmptyDataAccesstor<object, object>.Instance.FindAsync((object)null));
            Assert.ThrowsException<NotImplementedException>(() => EmptyDataAccesstor<object, object>.Instance.FindAsync((IReadOnlyList<object>)null));
        }
    }
}
