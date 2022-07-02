using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.HL.Redis.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CompressionSettingAttribute : Attribute
    {
        public string EncodingName { get; set; }

        public CompressionLevel Level { get; set; }

        public Encoding Encoding => EncodingName == null ? Encoding.UTF8 : Encoding.GetEncoding(EncodingName);
    }
}
