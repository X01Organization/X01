using System.Reflection;
using System.Text.Json;
using X01.CmdLine;
using X01.Core.Extensions;

namespace X01.FileLinq;
public static class FileLinqRunner
{
    public static async Task RunAsync(string[] args, CancellationToken token)
    {
        FileLinqOption option = new CmdLineArgsParser().Parse<FileLinqOption>(args);
        string? callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
        //await new Reporter(option).ReportAsync(default);
        if ("json".Equals(option.Format, StringComparison.OrdinalIgnoreCase))
        {
        }
        else
        {

            foreach (string action in option.Actions!)
            {
                IAsyncEnumerable<string> lines = File.ReadLinesAsync(option.SourceFile!, token);
                AsyncEnumerableExtension.InvokeAsyncEnumerableMethod(lines, typeof(string), action);
            }
        }
    }
}
