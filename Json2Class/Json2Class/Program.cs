using CommandLine;
using Json2Class;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o => { new Json2ClassConverter(o).DoJobAsync().GetAwaiter().GetResult(); });