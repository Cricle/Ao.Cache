using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class EntityConvertorBaseTest
    {
        [ExcludeFromCodeCoverage]
        class NullEntityConvertor : IEntityConvertor
        {
            public byte[] ToBytes(object entry, Type type)
            {
                return new byte[0];
            }

            public object ToEntry(byte[] bytes, Type type)
            {
                return null;
            }

            public object ToEntry(ReadOnlyMemory<byte> bytes, Type type)
            {
                return null;
            }

            public object TransferFromString(string data, Type type)
            {
                return null;
            }

            public string TransferToString(object obj, Type type)
            {
                return null;
            }
        }
        [TestMethod]
        public void Conver()
        {
            var cov = new NullEntityConvertor();
            cov.ToBytes(1,typeof(int));
            cov.ToEntry(new byte[0], typeof(object));
        }
    }
}
