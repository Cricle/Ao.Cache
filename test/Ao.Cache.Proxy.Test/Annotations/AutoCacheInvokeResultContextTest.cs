using Ao.Cache.Proxy.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ao.Cache.Proxy.Model;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoCacheInvokeResultContextTest
    {
        [TestMethod]
        public void Init()
        {
            var r = new AutoCacheResult<object>();
            var ex = new Exception();
            var ctx = new AutoCacheInvokeResultContext<int>(10, r, ex);

            Assert.AreEqual(r, ctx.CacheResult);
            Assert.AreEqual(ex, ctx.Exception);
            Assert.AreEqual(10, ctx.Result);
        }
    }
}
