using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheRun();
            //r.Times = 20_000;
            //r.Concurrent = 1000;
            //r.Setup();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}