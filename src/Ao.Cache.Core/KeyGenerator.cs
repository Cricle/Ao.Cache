using System;
using System.Runtime.CompilerServices;

namespace Ao.Cache
{
    public static class KeyGenerator
    {
        public const string DefaultSplit = "_";

        public const string NullString = "(Null)";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(string header, params object[] parts)
        {
            return ConcatWithSplit(header, DefaultSplit, parts);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(string header, object part1)
        {
            return ConcatWithSplit(header, DefaultSplit, part1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(string header, object part1, object part2)
        {
            return ConcatWithSplit(header, DefaultSplit, part1, part2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(string header, object part1, object part2, object part3)
        {
            return ConcatWithSplit(header, DefaultSplit, part1, part2, part3);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(string header, object part1, object part2, object part3, object part4)
        {
            return ConcatWithSplit(header, DefaultSplit, part1, part2, part3, part4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatWithSplit(string header, string split, object part1)
        {
            return string.Concat(header, split, part1 ?? NullString);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatWithSplit(string header, string split, object part1, object part2)
        {
            return string.Concat(header, split, part1 ?? NullString, split, part2 ?? NullString);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatWithSplit(string header, string split, object part1, object part2, object part3)
        {
            return string.Concat(header, split, part1 ?? NullString, split, part2 ?? NullString, split, part3 ?? NullString);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatWithSplit(string header, string split, object part1, object part2, object part3, object part4)
        {
            return string.Concat(header, split, part1 ?? NullString, split, part2 ?? NullString, split, part3 ?? NullString, split, part4 ?? NullString);
        }
        public static string ConcatCopyWithSplit(string header, string split, object[] parts)
        {
            var cpParts = new object[parts.Length];
            Array.Copy(parts, cpParts,parts.Length);
            return ConcatWithSplit(header, split, ref cpParts);
        }
        public static string ConcatWithSplit(string header, string split,ref object[] parts)
        {
            if (parts is null)
            {
                throw new ArgumentNullException(nameof(parts));
            }
            for (int i = 0; i < parts.Length; i++)
            {
                var item = parts[i];
                if (item == null)
                {
                    parts[i] = NullString;
                }
            }
            return string.Concat(header, split, string.Join(split, parts));
        }

        [ThreadStatic]
        private static readonly Random random = new Random();

        public static TimeSpan GetEmitTime(in TimeSpan time)
        {
            return time.Add(TimeSpan.FromMilliseconds(random.Next(10, 100)));
        }
    }
}
