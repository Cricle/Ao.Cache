using System;

namespace Ao.Cache.Proxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AutoLockAttribute : Attribute
    {
        public static readonly TimeSpan DefaultExpireTime = TimeSpan.FromSeconds(10);

        public AutoLockAttribute()
        {
            ExpireTime = DefaultExpireTime;
        }
        public AutoLockAttribute(string expireTimeStr)
        {
            ExpireTime = TimeSpan.Parse(expireTimeStr);
        }
        public AutoLockAttribute(TimeSpan expireTime)
        {
            ExpireTime = expireTime;
        }

        public TimeSpan ExpireTime { get; }
    }
}
