using System;
using System.Threading.Tasks;

namespace Ao.Cache.Events
{
    public interface IEventReceiver<T>
    {
        Task OnReceivedAsync(string channel, T data);
    }
}
