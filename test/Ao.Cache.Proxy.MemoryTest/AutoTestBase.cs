using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Proxy.MemoryTest
{
    public abstract class AutoTestBase
    {
        protected IServiceProvider CreateProvider(Action<IContainer> action)
        {
            var ser = new ServiceCollection();
            ser.WithCastleCacheProxy();
            ser.AddInMemoryFinder();

            var icon = new Container(Rules.MicrosoftDependencyInjectionRules)
                .WithDependencyInjectionAdapter(ser, null, RegistrySharing.CloneAndDropCache);
            action?.Invoke(icon);
            return icon.BuildServiceProvider();

        }
    }
}
