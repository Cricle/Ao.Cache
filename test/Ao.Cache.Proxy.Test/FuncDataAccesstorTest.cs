using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class FuncDataAccesstorTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FuncDataAccesstor<object, object>(null));
        }
        [TestMethod]
        public async Task GivenAndCall()
        {
            var ac = new FuncDataAccesstor<object, object>(() =>
            {
                return Task.FromResult<object>(1);
            });
            var res=await ac.FindAsync(null);
            Assert.AreEqual(1, res);
        }
    }
}
