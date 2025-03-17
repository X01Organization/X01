namespace X01.CmdLine;
public sealed class IoCmdLineOption
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input file or directory")]
    public string? InputFileOrDirectory { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output file or directory")]
    public string? OutputFileOrDirectory { get; set; }
}

public static class IoCmdLineOptionExt
{
    public static async Task Loop(this IoCmdLineOption option, Func<FileInfo, FileInfo, Task> fileHandler)
    {
        if (string.IsNullOrWhiteSpace(option.InputFileOrDirectory))
        {
            throw new ArgumentException("No input file or directory specified");
        }

        string inputFileOrDirectory = option.InputFileOrDirectory;
        FileAttributes attr = File.GetAttributes(inputFileOrDirectory);
        if (attr.HasFlag(FileAttributes.Directory))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(inputFileOrDirectory);
            FileInfo[] infputFiles = directoryInfo.GetFiles("*", new EnumerationOptions()
            {
                MaxRecursionDepth = 10000,
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false,
            });
            foreach (FileInfo x in infputFiles)
            {
                await fileHandler.Invoke(x, x);
            }
        }
        else
        {
            await fileHandler.Invoke(new FileInfo(inputFileOrDirectory), new FileInfo(inputFileOrDirectory));
        }
    }
}
