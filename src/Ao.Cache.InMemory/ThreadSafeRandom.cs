//The MIT License (MIT)

//Copyright(c) 2018 Sam Cook

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Runtime.CompilerServices;

namespace Ao.Cache.InMemory
{
    //https://github.com/samcook/RedLock.net/blob/master/RedLockNet.SERedis/Util/ThreadSafeRandom.cs
    internal static class ThreadSafeRandom
    {
        private static readonly Random GlobalRandom = new Random();

        [ThreadStatic]
        private static Random localRandom;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next()
        {
            return GetLocalRandom().Next();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next(int maxValue)
        {
            return GetLocalRandom().Next(maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next(int minValue, int maxValue)
        {
            return GetLocalRandom().Next(minValue, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NextDouble()
        {
            return GetLocalRandom().NextDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NextBytes(byte[] buffer)
        {
            GetLocalRandom().NextBytes(buffer);
        }

        private static Random GetLocalRandom()
        {
            if (localRandom == null)
            {
                lock (GlobalRandom)
                {
                    if (localRandom == null)
                    {
                        var seed = GlobalRandom.Next();
                        localRandom = new Random(seed);
                    }
                }
            }

            return localRandom;
        }
    }
}
