using Ao.Cache.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test.Events
{
    [TestClass]
    public class DelegateEventReceiverTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateEventReceiver<object>(null));
        }
        [TestMethod]
        public void Call()
        {
            var entry = false;
            var rec = new DelegateEventReceiver<object>((x, y) =>
              {
                  entry = true;
                  return Task.CompletedTask;
              });
            rec.OnReceivedAsync(null, null);
            Assert.IsTrue(entry);
        }
    }
}
