﻿using System.Runtime.CompilerServices;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;

class Program
{
    static async Task Main(string[] args)
    {
        MSBuildLocator.RegisterDefaults();
        string solutionPath = "C:\\workroot\\env\\config\\vscode\\workspace\\ja\\sln\\all.sln";
        using MSBuildWorkspace workspace = MSBuildWorkspace.Create();
        Solution solution = await workspace.OpenSolutionAsync(solutionPath);
        if (workspace.Diagnostics.Any())
        {
            foreach (WorkspaceDiagnostic diag in workspace.Diagnostics)
                Console.WriteLine(diag.Message);
        }
        foreach (Project project in solution.Projects)
        {
            if (project.Name != "DotnetRestSvc")
            {
                continue;
            }

            Console.WriteLine($"Processing project: {project.Name}");

            Compilation? compilation = await project.GetCompilationAsync();

            foreach (Document document in project.Documents)
            {
                if (document.Name.EndsWith("Controller.cs", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                SyntaxNode? root = await document.GetSyntaxRootAsync();
                if (root == null)
                    continue;

                SemanticModel? model = await document.GetSemanticModelAsync();
                if (model == null)
                    continue;

                IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword));

                List<MethodDeclarationSyntax> toRemove = new();

                foreach (MethodDeclarationSyntax? method in methods)
                {
                    IMethodSymbol? symbol = model.GetDeclaredSymbol(method);
                    if (symbol == null)
                        continue;

                    bool hasReference = HasReferencesAsync(solution, symbol, CancellationToken.None).GetAwaiter().GetResult();
                    if (!hasReference)
                    {
                        Console.WriteLine($"[UNUSED] {symbol.ToDisplayString()}");
                    }

                    //var s = GetInterfaceMethods(symbol).ToArray();
                    //var containingType = symbol.ContainingType;

                    //if (symbol.ExplicitInterfaceImplementations.Length > 0)
                    //{
                    //    Console.WriteLine($"[EXPLICIT INTERFACE METHOD] {symbol.ToDisplayString()} implements:");
                    //    foreach (var ifaceMethod in symbol.ExplicitInterfaceImplementations)
                    //    {
                    //        Console.WriteLine($"    → {ifaceMethod.ContainingType.ToDisplayString()}::{ifaceMethod.Name}");
                    //    }
                    //}

                    //// Check if the containing type implements any interfaces
                    //if (containingType.Interfaces.Length > 0)
                    //{
                    //    Console.WriteLine($"[HAS INTERFACES] {containingType.Name} implements: " +
                    //        string.Join(", ", containingType.Interfaces.Select(i => i.ToDisplayString())));
                    //}
                    //else
                    //{
                    //    Console.WriteLine($"[NO INTERFACES] {containingType.Name}");
                    //}

                    //if (containingType.TypeKind == TypeKind.Interface)
                    //{
                    //    Console.WriteLine($"[INTERFACE METHOD] {symbol.ToDisplayString()}");
                    //}
                    //else if (containingType.TypeKind == TypeKind.Class)
                    //{
                    //    Console.WriteLine($"[CLASS METHOD] {symbol.ToDisplayString()}");
                    //}
                    //else
                    //{
                    //    Console.WriteLine($"[OTHER TYPE] {containingType.TypeKind} - {symbol.ToDisplayString()}");
                    //}
                    //var references = await SymbolFinder.FindReferencesAsync(symbol.OriginalDefinition, solution);

                    //var realReferences = references.Where(r => r.Locations.Any()).ToArray();
                    //if (!references.Any(r => r.Locations.Any()))
                    //{
                    //    Console.WriteLine($"[UNUSED] {symbol.ToDisplayString()}");
                    //}
                }

            }
        }

        Console.WriteLine("Done.");
    }

    private static async Task<bool> HasReferencesAsync(
        Solution solution,
        IMethodSymbol method,
        CancellationToken token)
    {
        await foreach (ReferencedSymbol reference in FindReferencesAsync(solution, method, token))
        {
            if (reference.Locations.Any())
            {
                return true;
            }
        }

        return false;
    }

    private static async IAsyncEnumerable<ReferencedSymbol> FindReferencesAsync(
        Solution solution,
        IMethodSymbol method,
        [EnumeratorCancellation] CancellationToken token)
    {
        IEnumerable<ReferencedSymbol> methodReferences = await SymbolFinder.FindReferencesAsync(method.OriginalDefinition, solution, token);
        foreach (ReferencedSymbol methodReference in methodReferences)
        {
            yield return methodReference;
        }

        foreach (IMethodSymbol interfaceMethod in GetInterfaceMethods(method))
        {
            await foreach (ReferencedSymbol interfaceMethodReference in FindReferencesAsync(solution, interfaceMethod, token))
            {
                yield return interfaceMethodReference;
            }
        }
    }

    private static IEnumerable<IMethodSymbol> GetInterfaceMethods(IMethodSymbol method)
    {
        INamedTypeSymbol containingType = method.ContainingType;
        foreach (INamedTypeSymbol inheritedInterfaceType in containingType.AllInterfaces)
        {
            foreach (IMethodSymbol interfaceMethod in inheritedInterfaceType.GetMembers().OfType<IMethodSymbol>().Cast<IMethodSymbol>())
            {
                yield return interfaceMethod;
                //var impl = containingType.FindImplementationForInterfaceMember(interfaceMethod) as IMethodSymbol;
                //if (impl != null && SymbolEqualityComparer.Default.Equals(impl, method))
            }
        }

    }
}
//if (toRemove.Count > 0)
//{
//    var newRoot = root.RemoveNodes(toRemove, SyntaxRemoveOptions.KeepNoTrivia);
//    File.WriteAllText(document.FilePath, newRoot.ToFullString());
//}


