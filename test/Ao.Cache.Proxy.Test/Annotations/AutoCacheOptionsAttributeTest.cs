﻿using Ao.Cache.Proxy.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test.Annotations
{
    [TestClass]
    public class AutoCacheOptionsAttributeTest
    {
        [TestMethod]
        public void Defualts()
        {
            var attr = new AutoCacheOptionsAttribute();

            Assert.AreEqual(AutoCacheOptionsAttribute.DefaultCacheTime, attr.CacheTime);
            Assert.AreEqual(AutoCacheOptionsAttribute.DefaultCanRenewal, attr.CanRenewal);
            Assert.AreEqual(AutoCacheOptionsAttribute.DefaultLock, attr.Lock);
            Assert.AreEqual(AutoCacheOptionsAttribute.DefaultLockTime, attr.LockTime);
            Assert.AreEqual(AutoCacheOptionsAttribute.DefaultRenewal, attr.Renewal);
        }
    }
}
