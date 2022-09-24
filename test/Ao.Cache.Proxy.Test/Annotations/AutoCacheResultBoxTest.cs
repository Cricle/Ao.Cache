using Ao.Cache.Proxy.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoCacheResultBoxTest
    {
        [TestMethod]
        public void EmptyState()
        {
            var box = new AutoCacheResultBox<object>();

            Assert.IsNull(box.Result);
            Assert.IsFalse(box.HasResult);
        }
        [TestMethod]
        public void HasResult()
        {
            var box = new AutoCacheResultBox<object>();
            box.SetResult(1);

            Assert.AreEqual(1,box.Result);
            Assert.IsTrue(box.HasResult);
        }
    }
}
