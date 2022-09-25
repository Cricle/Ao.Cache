using System.Collections;

namespace Ao.Cache.Events
{
    public class EventPublishResult
    {
        public EventPublishResult(bool succeed)
            : this(null, succeed)
        {
        }

        public EventPublishResult(IDictionary features, bool succeed)
        {
            Features = features;
            Succeed = succeed;
        }

        public IDictionary Features { get; }

        public bool Succeed { get; }
    }
}
