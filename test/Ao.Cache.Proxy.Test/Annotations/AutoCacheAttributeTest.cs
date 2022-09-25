using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using System.Reflection;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [ExcludeFromCodeCoverage]
    class NullServiceProvider : IServiceProvider
    {
        object? IServiceProvider.GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
    [ExcludeFromCodeCoverage]
    class NullInvocationInfo : IInvocationInfo
    {
        object[] IInvocationInfo.Arguments => throw new NotImplementedException();

        object IInvocationInfo.Target => throw new NotImplementedException();

        MethodBase IInvocationInfo.Method => throw new NotImplementedException();

        object IInvocationInfo.ReturnValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        Type IInvocationInfo.TargetType => throw new NotImplementedException();
    }
    [TestClass]
    public class AutoCacheAttributeTest
    {
        [TestMethod]
        public async Task DecorateAsync_FinderWillChanged()
        {
            var attr = new AutoCacheAttribute();
            var ctx = new AutoCacheDecoratorContext<object>(
                new NullInvocationInfo(),
                new NullServiceProvider(),
                new NullDataFinder<UnwindObject, object>(),
                new UnwindObject());
            await attr.DecorateAsync(ctx);

            Assert.IsInstanceOfType(ctx.DataFinder.Options, IgnoreHeadDataFinderOptions<object>.Options.GetType());
            Assert.AreEqual(int.MinValue, attr.Order);
        }
    }
}
