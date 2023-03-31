using Microsoft.CodeAnalysis;

namespace Ao.Cache.CodeGen
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor NotImplement = new DiagnosticDescriptor(
            "AOCACHE_0001",
            "Fail to generace",
            "The class must implement IDataAccesstor<T1,T2>",
            "AOCACHE", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor InterfaceProxyMustGivenProxyType = new DiagnosticDescriptor(
            "AOCACHE_0003",
            "Fail to proxy",
            "Interface proxy must given proxy type",
            "AOCACHE", DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor CacheTimeCanNotConvert = new DiagnosticDescriptor(
            "AOCACHE_0004",
            "Fail convert cache time",
            "The cache time must be HH:mm:SS",
            "AOCACHE", DiagnosticSeverity.Error, true);
        public static readonly DiagnosticDescriptor ReturnMustRefOrNullable = new DiagnosticDescriptor(
           "AOCACHE_0005",
           "Will fail to proxy with value return",
           "The return is struct, must only class or Nullable<> can be check is in cache!",
           "AOCACHE", DiagnosticSeverity.Warning, true);

    }
}
