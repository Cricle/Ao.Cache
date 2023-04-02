using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheVsEasyCaching();
            //r.Times = 100;
            //r.Concurrent = 100;
            //r.IsUseRedis = true;
            //r.Setup().GetAwaiter().GetResult();
            //r.UseProxy().GetAwaiter().GetResult();
            //r.UseProvider().GetAwaiter().GetResult();
            //r.EasyCaching().GetAwaiter().GetResult();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}