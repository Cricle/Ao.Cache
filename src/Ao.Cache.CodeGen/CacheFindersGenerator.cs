using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ao.Cache.CodeGen
{

    [Generator]
    public class CacheFindersGenerator : IIncrementalGenerator
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
                        return (ClassDeclarationSyntax)ctx.Node;
                    }
                    return null;
                }).Where(x=>x !=null);
            context.RegisterSourceOutput(c, Execute);
            context.RegisterPostInitializationOutput(ctx =>
            {
                var sourceText = $@"
using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Gen
{{
    internal partial class DataFinders
    {{
        protected readonly IServiceProvider provider;

        public DataFinders(IServiceProvider provider)
        {{
            this.provider = provider;
        }}        
     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IDataFinder<TIdentity, TEntity> Get<TIdentity, TEntity>(TimeSpan? cacheTime = null, bool renewal = false)
        {{
            var finder = provider.GetRequiredService<IDataFinder<TIdentity, TEntity>>();
            if (cacheTime != null)
            {{
                finder.Options.WithCacheTime(cacheTime);
                finder.Options.WithRenew(renewal);
            }}
            return finder;
        }}
    }}
}}
";
                ctx.AddSource("DataFinders.g.cs", sourceText);
            });
        }


        private void Execute(SourceProductionContext context, ClassDeclarationSyntax @class)
        {
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
            var gen1 = interfaceType.TypeArgumentList.Arguments[0];
            var gen2 = interfaceType.TypeArgumentList.Arguments[1];
            var className = @class.Identifier.ToString().Replace("DataAccesstor", string.Empty);
            var sourceText = $@"
using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Ao.Cache.Gen
{{

    internal partial class DataFinders
    {{
        public IDataFinder<{gen1}, {gen2}> Get{className}(TimeSpan? cacheTime = null, bool renewal = false)
        {{
            return Get<{gen1}, {gen2}>(cacheTime, renewal);
        }}
    }}
}}
";
            context.AddSource($"DataFinders.{className}.g.cs", sourceText);
        }

    }
}
