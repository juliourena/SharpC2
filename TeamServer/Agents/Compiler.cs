using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TeamServer.Agents
{
    public class Compiler
    {
        public class CompilationRequest
        {
            public string SourceDirectory { get; set; }
            public string ReferenceDirectory { get; set; }
            public DotNetVersion TargetDotNetVersion { get; set; }
            public OutputKind OutputKind { get; set; }
            public Platform Platform { get; set; }
            public string AssemblyName { get; set; }
            public List<Reference> References { get; set; }
        }

        public enum DotNetVersion
        {
            Net35,
            Net40
        }

        public class Reference
        {
            public string File { get; set; }
            public DotNetVersion Framework { get; set; }
            public bool Enabled { get; set; }
        }

        private class SourceSyntaxTree
        {
            public string FileName { get; set; }
            public SyntaxTree SyntaxTree { get; set; }
            public List<INamedTypeSymbol> UsedTypes { get; set; } = new List<INamedTypeSymbol>();
        }

        
        public static byte[] Compile(CompilationRequest request)
        {
            var sourceSyntaxTrees = Directory.GetFiles(request.SourceDirectory, "*.cs", SearchOption.AllDirectories)
                .Select(f => new SourceSyntaxTree { FileName = f, SyntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(f), new CSharpParseOptions()) })
                .ToList();

            var compilationTrees = sourceSyntaxTrees.Select(s => s.SyntaxTree).ToList();
            var options = new CSharpCompilationOptions(outputKind: request.OutputKind, optimizationLevel: OptimizationLevel.Release, platform: request.Platform);

            var compilation = CSharpCompilation.Create(
                request.AssemblyName,
                compilationTrees,
                request.References.Where(r => r.Framework == request.TargetDotNetVersion).Where(r => r.Enabled).Select(r =>
                {
                    return MetadataReference.CreateFromFile(request.ReferenceDirectory + Path.DirectorySeparatorChar + r.File);
                }).ToList(),
                options
            );

            EmitResult emitResult;
            byte[] ILbytes = null;
            using (var ms = new MemoryStream())
            {
                emitResult = compilation.Emit(ms);
                if (emitResult.Success)
                {
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    ILbytes = ms.ToArray();
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (var d in emitResult.Diagnostics)
                    {
                        sb.AppendLine(d.ToString());
                    }
                    throw new CompilerException("CompilationErrors: " + Environment.NewLine + sb);
                }
            }
            return ILbytes;
        }

        private static string GetFullyQualifiedContainingNamespaceName(INamespaceSymbol namespaceSymbol)
        {
            var name = namespaceSymbol.Name;
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
            while (namespaceSymbol != null)
            {
                name = namespaceSymbol.Name + "." + name;
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }
            return name.Trim('.');
        }

        private static string GetFullyQualifiedContainingNamespaceName(INamedTypeSymbol namedTypeSymbol)
        {
            return GetFullyQualifiedContainingNamespaceName(namedTypeSymbol.ContainingNamespace);
        }

        private static string GetFullyQualifiedTypeName(INamedTypeSymbol namedTypeSymbol)
        {
            return GetFullyQualifiedContainingNamespaceName(namedTypeSymbol) + "." + namedTypeSymbol.Name;
        }

        private static List<INamedTypeSymbol> GetUsedTypes(CSharpCompilation compilation, SyntaxTree sourceTree)
        {
            return sourceTree.GetRoot().DescendantNodes().Select(N =>
            {
                var symbol = compilation.GetSemanticModel(sourceTree).GetSymbolInfo(N).Symbol;
                if (symbol != null && symbol.ContainingType != null)
                {
                    return symbol.ContainingType;
                }
                return null;
            }).Distinct().Where(T => T != null).ToList();
        }

        private static List<INamedTypeSymbol> GetUsedTypesRecursively(CSharpCompilation compilation, SyntaxTree sourceTree, ref List<INamedTypeSymbol> currentUsedTypes, ref List<SourceSyntaxTree> sourceSyntaxTrees)
        {
            var copyCurrentUsedTypes = currentUsedTypes.Select(CT => GetFullyQualifiedTypeName(CT)).ToList();
            var usedTypes = GetUsedTypes(compilation, sourceTree)
                .Where(T => !copyCurrentUsedTypes.Contains(GetFullyQualifiedTypeName(T)))
                .ToList();
            currentUsedTypes.AddRange(usedTypes);
            {
                
                return currentUsedTypes;
            }
        }

        public class CompilerException : Exception
        {
            public CompilerException()
            {

            }
            public CompilerException(string message) : base(message)
            {

            }
            public CompilerException(string message, Exception inner) : base(message, inner)
            {

            }
        }
    }
}
