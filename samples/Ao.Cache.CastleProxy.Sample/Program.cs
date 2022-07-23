using Microsoft.Extensions.DependencyInjection;
using DryIoc.Microsoft.DependencyInjection;
using System;
using Structing.DryInterceptor.Annotations;
using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.CastleProxy.Annotations;
using DryIoc;
using Ao.Cache.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Ao.Cache.CastleProxy.Model;

namespace Ao.Cache.CastleProxy.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ser = new ServiceCollection();
            ser.AddSingleton<GetTime>();
            ser.AddCastleCacheProxy();
            ser.AddMemoryCache();
            ser.AddSingleton(typeof(IDataFinder<,>), typeof(DefaultInMemoryCacheFinder<,>));

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            icon.AsyncIntercept(typeof(GetTime), typeof(CacheInterceptor));
            var provider = icon.BuildServiceProvider();
            var gt = provider.GetRequiredService<GetTime>();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                var n = gt.NowTime();
                Console.WriteLine($"Data:{n.RawData!.Value:HH:mm:ss ffff}, Status:{n.Status}");
            }
        }
    }
    public class GetTime
    {
        [AutoCache]
        public virtual AutoCacheResult<DateTime?> NowTime()
        {
            Console.WriteLine("yerp");
            return new AutoCacheResult<DateTime?> { RawData = DateTime.Now };
        }
    }
}