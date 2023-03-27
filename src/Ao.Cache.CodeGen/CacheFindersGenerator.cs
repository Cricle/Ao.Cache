using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ao.Cache.CodeGen
{
    [Generator]
    public class CacheFindersGenerator : CacheGenerator
    {
        protected override void Execute(SourceProductionContext context, ClassDeclarationSyntax ax, GeneratorSyntaxContext syntaxContext)
        {
            var rep = base.LookupDataAccesstor(ax,syntaxContext.SemanticModel);
            if (rep.ErrorDiagnostic!=null)
            {
                context.ReportDiagnostic(rep.ErrorDiagnostic);
                return;
            }
            var sourceText = $@"
using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace {(rep.NameSpace ?? TypeConsts.DefaultNameSpace)}
{{
    public static class {rep.ClassNameTrim}DataFindersExtensions
    {{
        public static IDataFinder<{rep.TypeArg1}, {rep.TypeArg2}> Get{rep.ClassNameTrim}(this DataFinders finders, TimeSpan? cacheTime = null, bool renewal = false)
        {{
            return finders.Get<{rep.TypeArg1}, {rep.TypeArg2}>(cacheTime, renewal);
        }}
    }}
}}
";
            context.AddSource($"DataFinders.{rep.ClassNameTrim}.g.cs", sourceText);
        }


    }
}
