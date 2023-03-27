using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheVsEasyCaching();
            //r.Times = 1;
            //r.Concurrent = 1;
            //r.Setup().GetAwaiter().GetResult();
            //r.UseProxy().GetAwaiter().GetResult();
            //r.UseProvider().GetAwaiter().GetResult();
            //r.EasyCaching().GetAwaiter().GetResult();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}