using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ao.Cache.CodeGen
{
    public class DataAccesstorLookup
    {
        public Diagnostic ErrorDiagnostic { get; set; }

        public GenericNameSyntax InterfaceType { get; set; }

        public ISymbol TypeArg1 { get; set; }

        public ISymbol TypeArg2 { get; set; }

        public INamedTypeSymbol Target { get; set; }

        public string NameSpace { get; set; }

        public string ClassName { get; set; }

        public string ClassNameTrim => ClassName == null ? null : ClassName.Replace("DataAccesstor", string.Empty);
    }
}
