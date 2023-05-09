﻿using Ao.Cache.Core.Annotations;
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
                            .AddSingleton<NowService>()
                            .AddInMemoryFinder()
                            .BuildServiceProvider();

            var p = provider.GetRequiredService<NowService>();

            var gc = GC.GetTotalMemory(true);
            var sw = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1_000_000; i++)
            {
                _ = p.Now(0).Result;
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
    public class NowService
    {
        public virtual Task<DateTime?> Now(int? add)
        {
            return Task.FromResult<DateTime?>(DateTime.Now.AddMilliseconds(add ?? 0));
        }
    }
}