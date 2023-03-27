using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Linq;

namespace Ao.Cache.CodeGen
{
    public abstract class CacheGenerator : IIncrementalGenerator
    {
        private IncrementalGeneratorInitializationContext context;

        protected IncrementalGeneratorInitializationContext Context=> context;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            this.context = context;
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
               (sn, tk) => sn is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
               (ctx, tk) =>
               {
                   var cq = ctx.SemanticModel;
                   var isDataAccesstor = cq.GetDeclaredSymbol(ctx.Node).GetAttributes().Any(x => TypeConsts.DataAccesstorAttribute.Equals(x.AttributeClass?.ToString()));
                   if (isDataAccesstor)
                   {
                       return new Tuple<ClassDeclarationSyntax,GeneratorSyntaxContext>((ClassDeclarationSyntax)ctx.Node,ctx);
                   }
                   return null;
               }).Where(x => x != null);
            context.RegisterSourceOutput(syntaxProvider, (ctx,source)=>Execute(ctx,source.Item1,source.Item2));
            OnInitialize(context);
        }

        protected virtual void OnInitialize(IncrementalGeneratorInitializationContext context)
        {

        }

        protected abstract void Execute(SourceProductionContext context, ClassDeclarationSyntax ax, GeneratorSyntaxContext syntaxContext);


        protected DataAccesstorLookup LookupDataAccesstor(ClassDeclarationSyntax @class,SemanticModel model)
        {
            var genType1 = @class.BaseList?.Types
                .Where(t =>
                {
                    if (t.Type is GenericNameSyntax genName)
                    {
                        var symbol = model.GetSymbolInfo(t.Type as GenericNameSyntax).Symbol;
                        return symbol != null && symbol.ToString().StartsWith(TypeConsts.IDataAccesstorName) && genName.TypeArgumentList.Arguments.Count == 2;
                    }
                    return false;
                })
                .FirstOrDefault();
            if (genType1 == null)
            {
                return new DataAccesstorLookup 
                { 
                    ErrorDiagnostic = Diagnostic.Create(DiagnosticDescriptors.NotImplement, Location.Create(@class.SyntaxTree, @class.Span))
                };
            }
            var interfaceType = (GenericNameSyntax)genType1.Type;
            var gen1 = model.GetSymbolInfo(interfaceType.TypeArgumentList.Arguments[0]).Symbol;
            var gen2 = model.GetSymbolInfo(interfaceType.TypeArgumentList.Arguments[1]).Symbol;
            var target = model.GetDeclaredSymbol(@class);
            var className = @class.Identifier.ToString();
            var attr = target.GetAttributes().Where(x => x.AttributeClass?.ToString().StartsWith(TypeConsts.DataAccesstorAttribute) ?? false).First();
            var nameSpacePair=attr.NamedArguments.FirstOrDefault(x => x.Key == TypeConsts.IDataAccesstorNameSpaceName);
            var nameSpace = nameSpacePair.Key != null ? nameSpacePair.Value.Value?.ToString() : null;
            return new DataAccesstorLookup
            {
                ClassName = className,
                InterfaceType = interfaceType,
                TypeArg1 = gen1,
                TypeArg2 = gen2,
                Target = target,
                NameSpace = nameSpace
            };
        }
    }
}
