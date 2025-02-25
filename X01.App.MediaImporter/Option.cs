using X01.CmdLine;

namespace X01.App.MediaImporter;
public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input directories or files")]
    public List<string>? InputFilesOrDirectories { get;set;}

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output directory")]
    public string? OutputDirectory { get;set;}

    [CmdLineArgs(ShortName = "e", LongName = "extension", HelpText = "the extension filter")]
    public List<string>? Extensions { get;set;}
}
