using Ao.Cache.Core.Annotations;
using Ao.Cache.Gen;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ao.Cache.Sample.CodeGenAot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                            //.AddSingleton<NowService>()
                            .AddSingleton<NowService, NowServiceProxy>()
                            .AddInMemoryFinder()
                            .BuildServiceProvider();

            var p = provider.GetRequiredService<NowService>();
            var gc = GC.GetTotalMemory(true);
            var sw = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1_000_000; i++)
            {
                _ = p.NowSync(i%1000,null,null);
            }
            Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp()-sw));
            Console.WriteLine($"{(GC.GetTotalMemory(false)-gc)/1024/1024.0}MB");
        }
    }
    //[CacheProxy(ProxyType =typeof(NowService),EndName ="Proxy1")]
    interface INowService
    {
        DateTime? NowSync(int? add, object? a, object? c);

        ValueTask<DateTime?> Now(int? add, object? a, object? c);
    }
    [CacheProxy]
    class NowService:INowService
    {
        public virtual DateTime? NowSync(int? add, object? a, object? c)
        {
            return DateTime.Now.AddMilliseconds(add ?? 0);
        }
        public virtual ValueTask<DateTime?> Now(int? add,object? a,object? c)
        {
            return new ValueTask<DateTime?>(DateTime.Now.AddMilliseconds(add ?? 0));
        }
    }
}