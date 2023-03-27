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

        public static readonly DiagnosticDescriptor ProxyParaNo1 = new DiagnosticDescriptor(
            "AOCACHE_0002",
            "Fail to proxy",
            "The method args must only 1",
            "AOCACHE", DiagnosticSeverity.Error, true);
    }
}
