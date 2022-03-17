using StackExchange.Redis;
using Microsoft.IO;

namespace Ao.Cache.Redis.Converters
{
    internal static class SharedMemoryStream
    {
        public static readonly RecyclableMemoryStreamManager StreamManager=new RecyclableMemoryStreamManager();
    }
}
