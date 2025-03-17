using X01.CmdLine;

namespace X01.App.CmdLine.ArgsParser.Test;
public  class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input files")]
    public List<int>? InputFiles { get;set;}

    public string? OutputFile { get;set;}
}
