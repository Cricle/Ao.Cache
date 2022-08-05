using System;
using System.Runtime.CompilerServices;

namespace Ao.Cache.CastleProxy.Interceptors
{
    internal static class ArrayMaker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Make<T>(int length)
        {
#if NET5_0_OR_GREATER
            return GC.AllocateUninitializedArray<T>(length);
#else
            return new T[length];
#endif
        }
    }
}
