using Ao.Cache.Proxy.Interceptors;

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
