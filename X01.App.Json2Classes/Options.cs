namespace Json2Class
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "the input json file")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "the output directory")]
        public string OutputDirectory { get; set; }

        [Option('n', "classname", Required = true, HelpText = "the class name")]
        public string ClassName { get; set; }

        [Option('s', "namespace", Required = true, HelpText = "the namespace")]
        public string Namespace { get; set; }
    }
}