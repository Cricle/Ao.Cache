using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;
using System;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheRun();
            //r.Times = 10000;
            //r.Setup();

            //r.HasResult();
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}