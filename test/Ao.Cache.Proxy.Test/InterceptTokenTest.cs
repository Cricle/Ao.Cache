using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class InterceptTokenTest
    {
        [TestMethod]
        public void StaticCheck()
        {
            var res = InterceptToken<object>.ActualTypeInfos;

            Assert.AreEqual(typeof(object), res.ActualType);
        }
    }
}
