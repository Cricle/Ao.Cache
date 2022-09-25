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
        class NullEntityConvertor<TEntity> : EntityConvertorBase<TEntity>
        {
            public override byte[] ToBytes(object entry, Type type)
            {
                return new byte[0];
            }

            public override object ToEntry(byte[] bytes, Type type)
            {
                return default(TEntity);
            }
        }
        [TestMethod]
        public void Conver()
        {
            var cov = new NullEntityConvertor<int>();
            cov.ToBytes(1);
            cov.ToEntry(new byte[0]);
        }
    }
}
