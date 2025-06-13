using X01.CmdLine;

namespace X01.App.TrimDistinct;

public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input file")]
    public string? Input { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output file")]
    public string? Output { get; set; }
}
