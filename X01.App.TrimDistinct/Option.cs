using X01.CmdLine;

namespace X01.App.TrimDistinct;

public class Option
{
    [CmdLineArgs(Required = true,ShortName = "i", LongName = "input", HelpText = "the input file")]
    public required string Input { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output file")]
    public string? Output { get; set; }
}
