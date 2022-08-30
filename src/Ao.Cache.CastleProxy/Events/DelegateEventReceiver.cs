using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Events
{
    public class DelegateEventReceiver<T> : IEventReceiver<T>
    {
        public DelegateEventReceiver(Func<string, T, Task> call)
        {
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public Func<string, T, Task> Call { get; }

        public Task OnReceivedAsync(string channel, T data)
        {
            return Call(channel, data);
        }
    }
}
