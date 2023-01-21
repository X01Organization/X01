using CommandLine;

namespace BlowFishDecoder
{
    public class Options
    {
        [Option('k', "key", Required = true, HelpText = "BlowFish-key required!")]
        public string Key { get; set; }

        [Option('d', "data", Required = true, HelpText = "BlowFish-data required!")]
        public string Data { get; set; }

        [Option('l', "len", Required = true, HelpText = "BlowFish-data-len required!")]
        public int Len { get; set; }
    }
}