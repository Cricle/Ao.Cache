using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.Proxy.Annotations;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Proxy.MemoryTest
{
    [TestClass]
    public class LockCaseTest : AutoTestProvider
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
                tasks[i] = await Task.Factory.StartNew(() => addSer.Add(10));
            }
            await Task.WhenAll(tasks);

            var exp = tasks.Length * 10;
            Assert.AreEqual(exp, addSer.Sum);
        }
        [TestMethod]
        public async Task BoundLockAdd()
        {
            var provider = CreateProvider(x =>
            {
                x.Register<BoundAddService>();
            });

            var addSer = provider.GetRequiredService<BoundAddService>();

            var tasks = new Task[100];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = await Task.Factory.StartNew(() => addSer.Add(10));
            }
            await Task.WhenAll(tasks);

            var exp = tasks.Length * 10;
            Assert.AreEqual(exp, addSer.Sum);
        }

    }
    public class BoundAddService
    {
        public int Sum { get; set; }

        [AutoLock]
        [global::Ao.Cache.MethodBoundaryAspect.Interceptors.LockInterceptor]
        public async Task Add(int count)
        {
            await Task.Yield();
            for (int i = 0; i < count; i++)
            {
                Sum++;
            }
            await Task.Yield();
        }
    }
}
