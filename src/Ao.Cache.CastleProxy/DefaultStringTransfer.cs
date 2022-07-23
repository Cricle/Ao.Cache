
/* 项目“Ao.Cache.CastleProxy (net5.0)”的未合并的更改
在此之前:
using System.Runtime.CompilerServices;
在此之后:
using Ao;
using Ao.Cache;
using Ao.Cache.CastleProxy;
using Ao.Cache.CastleProxy;
using Ao.Cache.CastleProxy.Interceptors;
using System.Runtime.CompilerServices;
*/
using System.Runtime.CompilerServices;

namespace Ao.Cache.CastleProxy
{
    public class DefaultStringTransfer : IStringTransfer
    {
        public const string DefaultSpliter = ".";

        public static readonly DefaultStringTransfer Instance = new DefaultStringTransfer();

        private DefaultStringTransfer() { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string CaseString(object val)
        {
            if (val == null)
            {
                return string.Empty;
            }
            return val.ToString();
        }
        public string Combine(params object[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return CaseString(args[0]);
                case 2:
                    return string.Concat(CaseString(args[0]), DefaultSpliter, CaseString(args[1]));
                default:
                    return string.Join(DefaultSpliter, args);
            }
        }

        public string ToString(object data)
        {
            return data?.ToString() ?? string.Empty;
        }
    }
}
