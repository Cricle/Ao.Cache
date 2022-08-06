using System;
using System.IO.Compression;
using System.Text;

namespace Ao.Cache.InRedis.HashList.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CompressionSettingAttribute : Attribute
    {
        public string EncodingName { get; set; }

        public CompressionLevel Level { get; set; }

        public Encoding Encoding => EncodingName == null ? Encoding.UTF8 : Encoding.GetEncoding(EncodingName);
    }
}
