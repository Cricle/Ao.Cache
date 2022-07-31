using System;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AutoCacheAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class AutoCacheSkipPartAttribute:Attribute
    {

    }

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
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class AutoLockSkipPartAttribute : Attribute
    {

    }
}
