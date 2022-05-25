using StackExchange.Redis;
using Ao.Cache.Redis.Annotations;
using System;
using System.IO.Compression;
using System.Reflection;
using System.IO;

namespace Ao.Cache.Redis.Converters
{
    internal static class CompressionHelper
    {
        private static readonly CompressionSettingAttribute defaultGzipSetting = new CompressionSettingAttribute();

        public static CompressionSettingAttribute GetAttribute(ICacheColumn column)
        {
            return column?.Property.GetCustomAttribute<CompressionSettingAttribute>() ?? defaultGzipSetting;
        }
        public static byte[] Gzip(byte[] buffer, CompressionLevel level)
        {
            return Gzip(buffer, 0, buffer.Length, level);
        }
        public static byte[] Gzip(byte[] buffer,int pos,int size, CompressionLevel level)
        {
            using (var s1 = new MemoryStream())
            using (var gs = new GZipStream(s1, level))
            {
                gs.Write(buffer, pos, size);
                gs.Flush();
                return s1.ToArray();
            }
        }
        public static byte[] UnGzip(byte[] buffer)
        {
            return UnGzip(buffer, 0, buffer.Length);
        }
        public static byte[] UnGzip(byte[] buffer, int pos, int size)
        {
            using (var s = new MemoryStream(buffer,pos,size))
            using (var s1 = new MemoryStream())
            using (var gs = new GZipStream(s, CompressionMode.Decompress))
            {
                gs.CopyTo(s1);
                return s1.ToArray();
            }
        }

        public static byte[] Deflate(byte[] buffer, CompressionLevel level)
        {
            return Deflate(buffer, 0, buffer.Length, level);
        }
        public static byte[] Deflate(byte[] buffer, int pos, int size, CompressionLevel level)
        {
            using (var s1 = new MemoryStream())
            using (var gs = new DeflateStream(s1, level))
            {
                gs.Write(buffer, pos, size);
                gs.Flush();
                return s1.ToArray();
            }
        }
        public static byte[] UnDeflate(byte[] buffer)
        {
            return UnDeflate(buffer, 0, buffer.Length);
        }
        public static byte[] UnDeflate(byte[] buffer, int pos, int size)
        {
            using (var s = new MemoryStream(buffer,pos, size))
            using (var s1 = new MemoryStream())
            using (var gs = new DeflateStream(s, CompressionMode.Decompress))
            {
                gs.CopyTo(s1);
                return s1.ToArray();
            }
        }
    }
}
