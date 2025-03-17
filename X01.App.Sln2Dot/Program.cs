using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

internal class Program
{
    private static async Task Main(string[] args)
    {
        MSBuildWorkspace workspace = MSBuildWorkspace.Create();
        Solution solution = await workspace.OpenSolutionAsync(@"C:\workroot\env\config\vscode\workspace\ja\sln\all.sln");

        List<string> callGraph = new();

        foreach (Project project in solution.Projects)
        {
            foreach (Document document in project.Documents)
            {
                SyntaxNode? root = await document.GetSyntaxRootAsync();

                // Look for method calls in the code
                IEnumerable<InvocationExpressionSyntax> methodInvocations = root!.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>();

                foreach (InvocationExpressionSyntax invocation in methodInvocations)
                {
                    string methodName = invocation.Expression.ToString();
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

