using CommandLine;
using ffind;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o => { new DuplicatedFileFinder(o).DoJob(); });
