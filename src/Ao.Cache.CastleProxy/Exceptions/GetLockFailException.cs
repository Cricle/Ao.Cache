using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
