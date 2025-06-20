using X01.CmdLine;

namespace X01.App.TrimDistinct;

public class Option
{
    [CmdLineArgs(Required = true,ShortName = "i", LongName = "input", HelpText = "the input file")]
    public required string Input { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output file")]
    public string? Output { get; set; }

    [CmdLineArgs(ShortName = "s", LongName = "separator", HelpText = "the separator to split the lines")]
    public string? Separator { get; set; }

    [CmdLineArgs(ShortName = "u", LongName = "upper case", HelpText = "convert to upper case")]
    public bool? UpperCase { get; set; }

    [CmdLineArgs(ShortName = "l", LongName = "lower case", HelpText = "convert to lower case")]
    public bool? LowerCase { get; set; }
}
