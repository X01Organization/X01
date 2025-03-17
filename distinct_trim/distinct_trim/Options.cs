namespace distinct_trim
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "output filename")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "output filename")]
        public string Output { get; set; } = "default";
    }
}