using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ao.Cache.CodeGen
{
    [Generator]
    public class CacheServiceRegistGenerator :  CacheGenerator
    {
        protected override void Execute(SourceProductionContext context, ClassDeclarationSyntax ax, GeneratorSyntaxContext syntaxContext)
        {
            var rep = base.LookupDataAccesstor(ax, syntaxContext.SemanticModel);
            if (rep.ErrorDiagnostic != null)
            {
                context.ReportDiagnostic(rep.ErrorDiagnostic);
                return;
            }

            var sourceText = $@"
using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace {(rep.NameSpace??TypeConsts.DefaultNameSpace)}
{{
    public static partial class DataFinderRegistServiceCollectionExtensions
    {{
        public static IServiceCollection Add{rep.ClassName}(this IServiceCollection services)
        {{
            services.AddScoped<IDataAccesstor<{rep.TypeArg1}, {rep.TypeArg2}>,{rep.Target}>();
            return services;
        }}
    }}
}}
";
            context.AddSource($"DataFindersServices.{rep.ClassName}.g.cs", sourceText);
        }
    }
}
