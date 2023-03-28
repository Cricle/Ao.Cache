using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.Cache.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple =false,Inherited = false)]
    public sealed class EnableCacheAutoServiceRegistAttribute:Attribute
    {
    }
}
