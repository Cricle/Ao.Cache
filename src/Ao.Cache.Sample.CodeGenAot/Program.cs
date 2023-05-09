using Ao.Cache.Core.Annotations;
using Ao.Cache.Gen;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ao.Cache.Sample.CodeGenAot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                            .AddSingleton<NowService,NowServiceProxy>()
                            .AddInMemoryFinder()
                            .BuildServiceProvider();

            var p = provider.GetRequiredService<NowService>();

            var gc = GC.GetTotalMemory(true);
            var sw = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1_000_000; i++)
            {
                _ = p.NowSync(0);
            }
            Console.WriteLine(Stopwatch.GetElapsedTime(sw));
            Console.WriteLine($"{(GC.GetTotalMemory(false)-gc)/1024/1024.0}MB");
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(p.Now(0)!.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
            //}
        }
    }
    [CacheProxy]
    class NowService
    {
        public virtual DateTime? NowSync(int? add)
        {
            return DateTime.Now.AddMilliseconds(add ?? 0);
        }
        public virtual Task<DateTime?> Now(int? add)
        {
            return Task.FromResult<DateTime?>(DateTime.Now.AddMilliseconds(add ?? 0));
        }
    }
}