using X01.CmdLine;

namespace X01.App.Json.Deserializer;
public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input file or directory")]
    public string? InputFileOrDirectory { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output file or directory")]
    public string? OutputFileOrDirectory { get; set; }
}
