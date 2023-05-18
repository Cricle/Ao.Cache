using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class DelegateSyncBatchDataAccesstorTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateSyncBatchDataAccesstor<string, object>(null));
        }
        [TestMethod]
        public void DelegateForAccesstor()
        {
            var map=new Dictionary<string, object>();
            var accesstor = new DelegateSyncBatchDataAccesstor<string, object>(_ => map);
            var actual = accesstor.Find(null);
            Assert.AreEqual(map, actual);
        }
    }
}
