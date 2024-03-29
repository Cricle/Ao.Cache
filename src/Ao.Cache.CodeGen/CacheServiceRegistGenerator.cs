﻿using Microsoft.CodeAnalysis;
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
/// <auto-generated>

using Ao.Cache;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace {(rep.NameSpace??TypeConsts.DefaultNameSpace)}
{{
    [System.Runtime.CompilerServices.CompilerGenerated]
    {InternalData.GeneratedCode}
    [System.Diagnostics.DebuggerStepThrough]
    public static partial class DataFinderRegistServiceCollectionExtensions
    {{
        [{TypeConsts.CacheForDataAccesstorAttribute}({TypeConsts.CacheForDataAccesstorServiceTypeAttribute}=typeof(IDataAccesstor<{rep.TypeArg1},{rep.TypeArg2}>),{TypeConsts.CacheForDataAccesstorImplementTypeAttribute}=typeof({rep.TypeArg2}))]
        public static IServiceCollection Add{rep.ClassName}(this IServiceCollection services)
        {{
            services.AddScoped<IDataAccesstor<{rep.TypeArg1}, {rep.TypeArg2}>,{rep.Target}>();
            return services;
        }}
    }}
}}
/// </auto-generated>
";
            context.AddSource($"DataFindersServices.{rep.ClassName}.g.cs", sourceText);
        }
    }
}
