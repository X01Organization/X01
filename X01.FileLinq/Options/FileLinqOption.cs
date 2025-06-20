using X01.CmdLine;

namespace X01.FileLinq.Options;

public class FileLinqOption
{
    [CmdLineArgs(ShortName = "s", LongName = "source", HelpText = "the source file")]
    public string? SourceFile { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "othersource", HelpText = "the other source file")]
    public string? OtherSourceFile { get; set; }

    [CmdLineArgs(ShortName = "r", LongName = "result", HelpText = "the result file")]
    public string? ResultFile { get; set; }

    [CmdLineArgs(ShortName = "a", LongName = "actions", HelpText = "the linq action: where? select:secornd file? distinct?")]
    public List<string>? Actions { get; set; }
}
