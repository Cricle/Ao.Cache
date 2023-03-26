using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Ao.Cache.CodeGen
{
    public class Ax
    {
        public ClassDeclarationSyntax Class { get; set; }

        public SemanticModel SemanticModel { get; set; }
    }
    [Generator]
    public class CacheServiceRegistGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var c = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, tk) => sn is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                (ctx, tk) =>
                {
                    var cq = ctx.SemanticModel;
                    var isDataAccesstor = cq.GetDeclaredSymbol(ctx.Node).GetAttributes().Any(x => "Ao.Cache.Core.Annotations.DataAccesstorAttribute".Equals(x.AttributeClass?.ToString()));
                    if (isDataAccesstor)
                    {
                        return new Ax
                        {
                              Class=(ClassDeclarationSyntax)ctx.Node,
                               SemanticModel=cq,
                        };
                    }
                    return null;
                }).Where(x => x != null);
            context.RegisterSourceOutput(c, Execute);
            context.RegisterPostInitializationOutput(ctx =>
            {
                var sourceText = $@"
using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Gen
{{
    public static partial class DataFinderRegistServiceCollectionExtensions
    {{
        public static IServiceCollection AddDataFinders(this IServiceCollection services)
        {{
            services.AddScoped<DataFinders>();
            return services;
        }}
    }}
}}
";
                ctx.AddSource("DataFindersServices.g.cs", sourceText);
            });
        }

        private void Execute(SourceProductionContext context, Ax ax)
        {
            var @class = ax.Class;
            var genType1 = @class.BaseList?.Types
                .Where(t => t.Type is GenericNameSyntax genName && genName.Identifier.Text.StartsWith("IDataAccesstor") && genName.TypeArgumentList.Arguments.Count == 2)
                .FirstOrDefault();
            if (genType1 == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                    "AOCACHE1", "Fail to generace", "The calss must implement IDataAccesstor<T1,T2>", "AOCACHE", DiagnosticSeverity.Error, true), null));
                return;
            }
            var interfaceType = (GenericNameSyntax)genType1.Type;
            var gen1 = ax.SemanticModel.GetSymbolInfo(interfaceType.TypeArgumentList.Arguments[0]).Symbol;
            var gen2 = ax.SemanticModel.GetSymbolInfo(interfaceType.TypeArgumentList.Arguments[1]).Symbol;
            var name = @class.Identifier.ToString().Replace("DataAccesstor", string.Empty);
            var ns = (@class.Parent as NamespaceDeclarationSyntax)?.Name?.ToString();
            var target = ax.SemanticModel.GetDeclaredSymbol(@class);
            var sourceText = $@"
using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Gen
{{
    public static partial class DataFinderRegistServiceCollectionExtensions
    {{
        public static IServiceCollection Add{name}DataAccesstor(this IServiceCollection services)
        {{
            services.AddScoped<IDataAccesstor<{gen1}, {gen2}>,{target}>();
            return services;
        }}
    }}
}}
";
            context.AddSource($"DataFindersServices.{name}.g.cs", sourceText);
        }

    }
}
