using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Events
{
    public interface IEventReceiver<T>
    {
        Task OnReceivedAsync(string channel, T data);
    }
}
