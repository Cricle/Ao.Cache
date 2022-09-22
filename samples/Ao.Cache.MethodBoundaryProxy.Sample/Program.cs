﻿using Ao.Cache.Events;
using Ao.Cache.MethodBoundaryAspect.Interceptors;
using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryProxy.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddCacheProxy();
            ser.AddInMemoryFinder();
            var provider = ser.BuildServiceProvider();
            GlobalMethodBoundary.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var finderFc = provider.GetRequiredService<AutoCacheService>();

            var gt = provider.GetRequiredService<GetTime>();
            for (int i = 0; i < 5; i++)
            {
                if (i%2==0)
                {
                    var re = finderFc.DeleteAsync<GetTime, DateTime?>(x => x.NowAsync()).GetAwaiter().GetResult();
                    Console.WriteLine($"DeleteResult: {re}");
                }
                var n = gt.NowAsync().GetAwaiter().GetResult();
                Console.WriteLine($"{n:HH:mm:ss.fffff}");
            }
        }
    }
    public class GetTime
    {
        [AutoCache]
        [CacheInterceptor]
        public Task<DateTime?> NowAsync()
        {
            return Task.FromResult<DateTime?>(DateTime.Now);
        }
    }
}