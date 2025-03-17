using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(@"C:\workroot\env\config\vscode\workspace\ja\sln\all.sln");

        List<string> callGraph = new List<string>();

        foreach (var project in solution.Projects)
        {
            foreach (var document in project.Documents)
            {
                var root = await document.GetSyntaxRootAsync();

                // Look for method calls in the code
                var methodInvocations = root!.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>();

                foreach (var invocation in methodInvocations)
                {
                    var methodName = invocation.Expression.ToString();
                    callGraph.Add(methodName);
                }
            }
        }

        // Output the call graph as a DOT file
        string dotFilePath = "call_graph.dot";
        string dotContent = "digraph CallGraph {\n" +
                         string.Join("\n", callGraph.Distinct().Select(line => $"\"{line}\" -> \"some_method\";")) +
                         "\n}";

        File.WriteAllText(dotFilePath, dotContent);

        Console.WriteLine($"Call graph saved to {dotFilePath}");
    }
}

