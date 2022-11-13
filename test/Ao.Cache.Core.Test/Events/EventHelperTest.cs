using Ao.Cache.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test.Events
{
    [TestClass]
    public class EventHelperTest
    {
        [TestMethod]
        public void GetChannelKey()
        {
            EventHelper.PrefxKey = "a";
            Assert.AreEqual("a", EventHelper.PrefxKey);

            var act = EventHelper.GetChannelKey<object>();
            Assert.AreEqual("a.System.Object", act);
            act = EventHelper.GetChannelKey<object>("aaa");
            Assert.AreEqual("aaa.System.Object", act);
        }
    }
}
