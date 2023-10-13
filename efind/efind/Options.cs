using CommandLine;

namespace efind
{
    public class Options
    {
        [Option('i', "inputs", Min = 1, Required = true, Separator = ';', HelpText = "input directories")]
        public IEnumerable<string> Inputs { get; set; }

        [Option('o', "output", Required = true, HelpText = "output directory")]
        public string Output { get; set; }
    }
}