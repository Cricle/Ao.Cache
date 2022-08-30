using System;
using System.Threading.Tasks;

namespace Ao.Cache.Events
{
    public interface IEventAdapter
    {
        Task<EventPublishResult> PublishAsync<T>(string channel, T data);

        Task<IDisposable> SubscribeAsync<T>(string channel, IEventReceiver<T> receiver);
    }
}
