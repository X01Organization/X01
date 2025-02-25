using X01.CmdLine;

namespace X01.FileLinq;

public class FileLinqOption
{
    [CmdLineArgs(ShortName = "s", LongName = "source", HelpText = "the source file")]
    public string? SourceFile { get; set; }

    [CmdLineArgs(ShortName = "r", LongName = "result", HelpText = "the result file")]
    public string? ResultFile { get; set; }

    [CmdLineArgs(ShortName = "f", LongName = "format", HelpText = "the file formats: json or list")]
    public string? Format { get; set; }

    [CmdLineArgs(ShortName = "a", LongName = "actions", HelpText = "the linq action: where? select:secornd file? distinct?")]
    public string[]? Actions { get; set; }
}
