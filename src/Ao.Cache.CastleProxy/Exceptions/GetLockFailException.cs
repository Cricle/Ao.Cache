using System;
using System.Runtime.Serialization;

namespace Ao.Cache.CastleProxy.Exceptions
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
        internal ILocker locker;

        public ILocker Locker => locker;
    }
}
