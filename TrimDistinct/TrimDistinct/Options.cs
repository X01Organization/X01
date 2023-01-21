using CommandLine;

namespace TrimDistinct
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "output file fullname")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "output file fullname")]
        public string Output { get; set; }
    }
}