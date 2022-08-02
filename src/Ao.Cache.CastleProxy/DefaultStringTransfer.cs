using System.Runtime.CompilerServices;

namespace Ao.Cache.CastleProxy
{
    public class DefaultStringTransfer : IStringTransfer
    {
        public const string DefaultSpliter = ".";

        public static readonly DefaultStringTransfer Default = new DefaultStringTransfer(DefaultSpliter);

        public DefaultStringTransfer(string spliter) 
        {
            Spliter = spliter ?? throw new System.ArgumentNullException(nameof(spliter));
        }


        public string Spliter { get; }

        public string Combine(params object[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return ToString(args[0]);
                case 2:
                    return string.Concat(ToString(args[0]), Spliter, ToString(args[1]));
                default:
                    return string.Join(Spliter, args);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(object data)
        {
            return data?.ToString() ?? string.Empty;
        }
    }
}
