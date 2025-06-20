using X01.CmdLine;

namespace X01.App.Linq;

public class Option
{
    [CmdLineArgs(Required = true, ShortName = "f", LongName = "1stfile", HelpText = "the 1st file")]
    public required string FirstSourceFile { get; set; }

    [CmdLineArgs(Required = true,ShortName = "s", LongName = "2ndfile", HelpText = "the 2nd file")]
    public required string SecondSourceFile { get; set; }

    [CmdLineArgs(ShortName = "r", LongName = "resultfile", HelpText = "the result file")]
    public string? ResultSourceFile { get; set; }
}
