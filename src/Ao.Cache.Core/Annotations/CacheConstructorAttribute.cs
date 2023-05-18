using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.Cache.Annotations
{
    [AttributeUsage(AttributeTargets.Constructor,AllowMultiple =false,Inherited = false)]
    public sealed class CacheConstructorAttribute:Attribute
    {
    }
}
