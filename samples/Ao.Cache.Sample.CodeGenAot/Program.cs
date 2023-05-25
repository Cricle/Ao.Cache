using Ao.Cache.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Ao.Cache.Sample.CodeGenAot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                            .AddSingleton<NowService, Gen.NowServiceProxy>()
                            .AddInMemoryFinder()
                            .BuildServiceProvider();
            Console.WriteLine("==== TestDelete");
            TestDelete(provider);
            Console.WriteLine("==== TestFinders");
            TestFinders(provider);
            Console.WriteLine("==== TestProxy");
            TestProxy(provider);
        }
        private static void TestDelete(IServiceProvider provider)
        {
            var p = provider.GetRequiredService<NowService>();
            var res = p.NowSync(0, null, null);
            Console.WriteLine("CurrentRes:" + res.Value.Ticks);
            var cacheHelper = provider.GetRequiredService<ICacheHelperCreator>();
            var incache = cacheHelper.FindInCache(() => p.NowSync(0, null, null));
            var ok = cacheHelper.Delete(() => p.NowSync(0, null, null));
            Console.WriteLine("Delete:" + ok);
            res = p.NowSync(0, null, null);
            Console.WriteLine("CurrentRes:" + res.Value.Ticks);
        }
        private static void TestProxy(IServiceProvider provider)
        {
            var p = provider.GetRequiredService<NowService>();
            var gc = GC.GetTotalMemory(true);
            var sw = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1_000_000; i++)
            {
                _ = p.NowSync(i % 1000, null, null);
            }
            Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp() - sw));
            Console.WriteLine($"{(GC.GetTotalMemory(false) - gc) / 1024 / 1024.0}MB");
        }
        private static void TestFinders(IServiceProvider provider)
        {
            var rand = new Random();
            var px = provider.GetRequiredService<IDataFinder<string, Student>>();
            var px1 = provider.GetRequiredService<ISyncDataFinder<string, Student>>();
            var r = px.FindAsync("aaabbbccc", new DelegateDataAccesstor<string, Student>(x => Task.FromResult(new Student { A = rand.Next(0, 9999) }))).Result;
            Console.WriteLine(r);
            r = px1.Find("aaabbbccc", new DelegateSyncDataAccesstor<string, Student>(x => new Student { A = rand.Next(0, 9999) }));
            Console.WriteLine(r);
        }
    }
}