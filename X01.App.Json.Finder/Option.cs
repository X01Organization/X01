using X01.CmdLine;

namespace X01.App.Json.Deserializer;
public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input file or directory")]
    public string? InputFileOrDirectory { get; set; }

    [CmdLineArgs(ShortName = "r", LongName = "regex", HelpText = "the regex to match the path")]
    public string? PathRegex { get; set; }
}
