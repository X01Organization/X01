using CommandLine;
using TrimDistinct;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o =>
       {
           File.WriteAllLines(o.Output!,
               File.ReadAllLines(o.Input!)
                   .Select(line => line.Trim())
                   .Where(line => !string.IsNullOrEmpty(line))
                   .Distinct()
                   .OrderBy(line => line));
       });