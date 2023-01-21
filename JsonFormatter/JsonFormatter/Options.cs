using CommandLine;

namespace JsonFormatter 
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "input json file name")]
        public string Input { get; set; }

        [Option('o', "output", HelpText = "output json file name")]
        public string Output { get; set; }
    }
}