using Ao.Cache.CastleProxy.Interceptors;
using Ao.Cache.Proxy.Annotations;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Autofac
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s = new ContainerBuilder();
            s.RegisterType<Services>()
                .SingleInstance()
                .InterceptedBy(typeof(CacheInterceptor)).EnableClassInterceptors();
            s.Populate(new ServiceCollection()
                .AddInMemoryFinder()
                .WithCastleCacheProxy()
                .AddScoped<CacheInterceptor>());
            var sss = s.Build().Resolve<Services>();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(778);
                Console.WriteLine(sss.Now()?.ToString("HH:mm:ss.fffff"));
            }
        }
    }
    [Intercept(typeof(CacheInterceptor))]
    public class Services
    {
        [AutoCache]
        [AutoCacheOptions("00:00:01")]
        public virtual DateTime? Now()
        {
            return DateTime.Now;
        }
    }
}