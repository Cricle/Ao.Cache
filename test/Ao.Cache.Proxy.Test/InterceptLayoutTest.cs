﻿using Ao.Cache.Proxy.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class InterceptLayoutTest
    {
        [AutoCache]
        [ExcludeFromCodeCoverage]
        class A
        {
        }
        [ExcludeFromCodeCoverage]
        class B
        {
            [AutoCache]
            public void Go()
            {

            }
            public void Go1()
            {

            }
        }
        [ExcludeFromCodeCoverage]
        class MyInvocationInfo : IInvocationInfo
        {
            public object[] Arguments { get; set; }

            public object Target { get; set; }

            public MethodBase Method { get; set; }

            public object ReturnValue { get; set; }

            public Type TargetType { get; set; }
        }
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            var ser = new ServiceCollection().BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            var nameHelper = DefaultCacheNamedHelper.Default;

            Assert.ThrowsException<ArgumentNullException>(() => new InterceptLayout(ser, null));
            Assert.ThrowsException<ArgumentNullException>(() => new InterceptLayout(null, nameHelper));
        }
        [TestMethod]
        public void CheckAuto()
        {
            var ser = new ServiceCollection().BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            var nameHelper = DefaultCacheNamedHelper.Default;
            var intercept = new InterceptLayout(ser, nameHelper);

            Assert.IsTrue(intercept.HasAutoCache(new MyInvocationInfo
            {
                TargetType = typeof(A)
            }));
            Assert.IsTrue(intercept.HasAutoCache(new MyInvocationInfo
            {
                TargetType = typeof(B),
                Method = typeof(B).GetMethod(nameof(B.Go))!
            }));
            Assert.IsFalse(intercept.HasAutoCache(new MyInvocationInfo
            {
                TargetType = typeof(B),
                Method = typeof(B).GetMethod(nameof(B.Go1))!
            }));
        }
    }
}
