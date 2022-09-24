using Ao.Cache.CastleProxy.Interceptors;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Proxy.MemoryTest
{
    [TestClass]
    public class LockCaseTest:AutoTestBase
    {
        [TestMethod]
        public async Task LockAdd()
        {
            var provider = CreateProvider(x =>
            {
                x.Register<AddService>();
                x.AsyncIntercept<AddService, LockInterceptor>();
            });

            var addSer = provider.GetRequiredService<AddService>();

            var tasks = new Task[100];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] =await Task.Factory.StartNew(() => addSer.Add(10));
            }
            await Task.WhenAll(tasks);

            var exp = tasks.Length * 10;
            Assert.AreEqual(exp, addSer.Sum);
        }
    }
}
