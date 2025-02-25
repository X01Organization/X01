using X01.CmdLine;

namespace Json2Classes;

public class Option
{
    [CmdLineArgs(ShortName = "i", LongName = "input", HelpText = "the input json file")]
        public string? InputJsonFile { get; set; }

    [CmdLineArgs(ShortName = "o", LongName = "output", HelpText = "the output directory for classes")]
        public string? OutputDirectory { get; set; }

    [CmdLineArgs(ShortName = "c", LongName = "classname", HelpText = "the class name")]
        public string? ClassName { get; set; }

    [CmdLineArgs(ShortName = "n", LongName = "namespace", HelpText = "the namespace")]
        public string? Namespace { get; set; }
}
