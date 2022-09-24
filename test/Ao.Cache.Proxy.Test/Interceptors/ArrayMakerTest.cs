using Ao.Cache.Proxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test.Interceptors
{
    [TestClass]
    public class ArrayMakerTest
    {
        [TestMethod]
        public void MakeArray()
        {
            var arr = ArrayMaker.Make<object>(0);
            Assert.AreEqual(0, arr.Length);

            arr = ArrayMaker.Make<object>(10);
            Assert.AreEqual(10, arr.Length);
        }
    }
}
