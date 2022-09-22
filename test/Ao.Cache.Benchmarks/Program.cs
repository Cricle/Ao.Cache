using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheRun();
            //r.Times = 200_000;
            //r.Concurrent = 10000;
            //r.Setup();
            //r.MethodBoundNoResult().GetAwaiter().GetResult();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}