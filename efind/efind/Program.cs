using CommandLine;
using efind;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o => { new EmptyFinder(o).DoJob(); });