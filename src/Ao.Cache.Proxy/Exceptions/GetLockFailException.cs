using System;
using System.Runtime.Serialization;

namespace Ao.Cache.Proxy.Exceptions
{
    public class GetLockFailException : Exception
    {
        public GetLockFailException()
        {
        }

        public GetLockFailException(string message) : base(message)
        {
        }

        public GetLockFailException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GetLockFailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ILocker Locker { get; set; }
    }
}
