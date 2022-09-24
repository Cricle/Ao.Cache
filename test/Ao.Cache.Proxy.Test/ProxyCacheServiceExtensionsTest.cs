using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class ProxyCacheServiceExtensionsTest
    {
        [TestMethod]
        public void AddServices()
        {
            var services = new ServiceCollection();
            ProxyCacheServiceExtensions.AddCacheProxy(services);

            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IStringTransfer)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(ICacheNamedHelper)));
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(AutoCacheService)));
        }
    }
}
