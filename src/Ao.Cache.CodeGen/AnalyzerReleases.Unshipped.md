; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AOCACHE_0001 | AOCACHE | Error | The class must implement IDataAccesstor<T1,T2>
AOCACHE_0003 | AOCACHE | Error | Interface proxy must given proxy type
AOCACHE_0004 | AOCACHE | Error | The cache time must be HH:mm:SS
AOCACHE_0005 | AOCACHE | Warning | The return is struct, must only class or Nullable<> can be check is in cache!