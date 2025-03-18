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

    private static string GetEnumeableGenericType(string callingAssemblyName)
    {
        return "";
    }

    private static async Task WriteToJsonFileAsync(IAsyncEnumerable<string> asyncEnumerable, string filePath)
        {
            // Create a FileStream to write to the JSON file
            await using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                // Create a StreamWriter to write JSON data to the FileStream
                await using (StreamWriter writer = new StreamWriter(stream))
                {
                    // Write the opening bracket of the JSON array
                    await writer.WriteAsync("[");

                    // Initialize a flag to determine if this is the first item in the JSON array
                    bool isFirstItem = true;

                    // Iterate over the async enumerable
                    await foreach (string item in asyncEnumerable)
                    {
                        // If this is not the first item, add a comma separator
                        if (!isFirstItem)
                            await writer.WriteAsync(",");

                    // Serialize the string item to JSON
                    string jsonString = JsonSerializer.Serialize(item);

                        // Write the JSON string to the file
                        await writer.WriteAsync(jsonString);

                        // Set the flag to false after the first item has been written
                        isFirstItem = false;
                    }

                    // Write the closing bracket of the JSON array
                    await writer.WriteAsync("]");
                }
            }
        }
    }
