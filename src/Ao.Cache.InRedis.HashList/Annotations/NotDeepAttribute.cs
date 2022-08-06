using System;

namespace Ao.Cache.InRedis.HashList.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class NotDeepAttribute : Attribute
    {
    }
}
