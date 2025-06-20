using System.Reflection;
using X01.CmdLine;
using X01.Core.Extensions;
using X01.FileLinq.Options;

namespace X01.FileLinq;
public static class FileLinqExecuter
{
    public static async Task ExecuteAsync (string[] args, CancellationToken token)
    {
        FileLinqOption option = new CmdLineArgsParser().Parse<FileLinqOption>(args);

            foreach (string action in option.Actions!)
            {
                IAsyncEnumerable<string> lines = File.ReadLinesAsync(option.SourceFile!, token);
                AsyncEnumerableExtension.InvokeAsyncEnumerableMethod(lines, typeof(string), action);
            }
    }
}
