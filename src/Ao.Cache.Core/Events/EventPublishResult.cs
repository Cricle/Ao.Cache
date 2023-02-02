using System.Collections;

namespace Ao.Cache.Events
{
    public readonly struct EventPublishResult
    {
        public EventPublishResult(bool succeed)
        {
            Succeed= succeed;
            Features = null;
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
