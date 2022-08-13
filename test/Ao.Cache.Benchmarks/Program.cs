﻿using Ao.Cache.Benchmarks.Actions;
using BenchmarkDotNet.Running;
using System;
using System.IO;

namespace Ao.Cache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var r = new AutoCacheRun();
            //r.Times = 10000;
            //r.Concurrent = 1000;
            //r.Setup();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
}