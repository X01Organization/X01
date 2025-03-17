using X01.CmdLine;

namespace X01.App.Ja.HttpQueryLogReporter;
public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input directory")]
    public string? InputOrDirectory { get;set;}

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output directory")]
    public string? OutputDirectory { get;set;}
}
